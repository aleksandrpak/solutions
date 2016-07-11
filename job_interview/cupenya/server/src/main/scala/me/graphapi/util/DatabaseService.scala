package me.graphapi.util

import java.util.concurrent.ConcurrentHashMap
import java.util.concurrent.atomic.AtomicInteger

import akka.NotUsed
import akka.stream.scaladsl.Source
import me.graphapi.model.Person
import me.graphapi.util.DatabaseService.PersonVertex

import scala.collection.JavaConverters._
import scala.collection.{concurrent, immutable}
import scala.concurrent.{ExecutionContext, Future}

object DatabaseService {

  type Followers = concurrent.Map[Int, Boolean]

  case class PersonVertex(person: Person, followers: Followers, deleted: Boolean)

}

class DatabaseService(implicit val executionContext: ExecutionContext) {

  private val idCounter = new AtomicInteger(0)
  private val personsIndex: concurrent.Map[Int, PersonVertex] = new ConcurrentHashMap[Int, PersonVertex]().asScala

  def hasPerson(id: Int): Future[Boolean] = Future {
    personsIndex.contains(id)
  }

  def getPersons: Source[Person, NotUsed] = {
    Source[Person](personsIndex.values.filter(!_.deleted).map(_.person).to[immutable.Iterable])
  }

  def getPerson(id: Int): Future[Option[Person]] = Future {
    personsIndex.get(id).flatMap { personVertex =>
      if (personVertex.deleted)
        None
      else
        Some(personVertex.person)
    }
  }

  def createPerson(person: Person): Future[Person] = Future {
    val id = idCounter.incrementAndGet()
    val followers = new ConcurrentHashMap[Int, Boolean]().asScala
    val newPerson = person.copy(id = Some(id))
    val personVertex = PersonVertex(newPerson, followers, deleted = false)

    personsIndex.putIfAbsent(id, personVertex) match {
      case Some(_) => throw new IllegalStateException("Reusing existing key is not allowed")
      case None => newPerson
    }
  }

  def deletePerson(id: Int): Future[Boolean] = Future {
    personsIndex.get(id).fold(false) { personVertex =>
      if (!personVertex.deleted)
        personsIndex.replace(id, personVertex, personVertex.copy(deleted = true))
      else
        false
    }
  }

  def getFollowers(id: Int): Source[Person, NotUsed] = {
    Source[Person](personsIndex.get(id).fold(immutable.Iterable.empty[Person]) { personVertex =>
      if (!personVertex.deleted)
        personVertex.followers.keys.flatMap(personsIndex.get(_).filter(!_.deleted).map(_.person)).to[immutable.Iterable]
      else
        immutable.Iterable.empty[Person]
    })
  }

  def addFollower(personId: Int, followerId: Int): Future[Boolean] = Future {
    personsIndex.get(personId).fold(false) { personVertex =>
      personsIndex.get(followerId).fold(false) { followerVertex =>
        if (personVertex.deleted || followerVertex.deleted) {
          false
        } else {
          personVertex.followers.putIfAbsent(followerId, false).getOrElse(true)
        }
      }
    }
  }

  def removeFollower(personId: Int, followerId: Int): Future[Boolean] = Future {
    personsIndex.get(personId).fold(false) { personVertex =>
      personVertex.followers.remove(followerId, false)
    }
  }

  def hasFollower(personId: Int, followerId: Int): Future[Boolean] = Future {
    personsIndex.get(personId).fold(false) { personVertex =>
      personVertex.followers.contains(followerId)
    }
  }
}
