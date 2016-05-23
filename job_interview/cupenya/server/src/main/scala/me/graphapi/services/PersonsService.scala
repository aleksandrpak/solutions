package me.graphapi.services

import akka.NotUsed
import akka.stream.scaladsl.Source
import me.graphapi.model.Person
import me.graphapi.util.DatabaseService

import scala.concurrent.{ExecutionContext, Future}

class PersonsService(databaseService: DatabaseService, followersGraphService: FollowersGraphService)
                    (implicit executionContext: ExecutionContext) {

  def getPersons: Source[Person, NotUsed] = databaseService.getPersons

  def getPerson(id: Int): Future[Option[Person]] = databaseService.getPerson(id)

  def createPerson(person: Person): Future[Person] = databaseService.createPerson(person)

  def deletePerson(id: Int): Future[Boolean] = databaseService.deletePerson(id)

  def getFollowers(id: Int): Source[Person, NotUsed] = databaseService.getFollowers(id)

  def addFollower(personId: Int, follower: Person): Future[Boolean] = {
    follower.id.fold(Future.successful(false)) { followerId =>
      if (personId == followerId)
        Future.successful(false)
      else
        databaseService.addFollower(personId, followerId)
    }
  }

  def removeFollower(personId: Int, followerId: Int): Future[Boolean] = {
    if (personId == followerId)
      Future.successful(false)
    else
      databaseService.removeFollower(personId, followerId)
  }

  def findRelation(personId: Int, relationId: Int): Future[List[Person]] = {
    followersGraphService.shortestPath(personId, relationId).flatMap { path =>
      Future.sequence(path.map(databaseService.getPerson)).map(_.flatten)
    }
  }
}
