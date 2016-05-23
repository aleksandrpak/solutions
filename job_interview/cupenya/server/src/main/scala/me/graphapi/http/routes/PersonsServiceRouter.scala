package me.graphapi.http.routes

import akka.http.scaladsl.model.StatusCodes._
import akka.http.scaladsl.server.Directives._
import akka.stream.ActorMaterializer
import akka.stream.scaladsl.Sink
import de.heikoseeberger.akkahttpcirce.CirceSupport
import io.circe.generic.auto._
import io.circe.syntax._
import me.graphapi.model.Person
import me.graphapi.services.PersonsService

import scala.concurrent.ExecutionContext

class PersonsServiceRouter(personsService: PersonsService)
                          (implicit val executionContext: ExecutionContext,
                           implicit val materializer: ActorMaterializer) extends CirceSupport {

  import personsService._

  val route = pathPrefix("persons") {
    pathEndOrSingleSlash {
      get {
        complete(getPersons.runWith(Sink.seq).map(_.asJson))
      } ~
        post {
          entity(as[Person]) { person =>
            complete(Created -> createPerson(person).map(_.asJson))
          }
        }
    } ~
      pathPrefix(IntNumber) { id =>
        pathEndOrSingleSlash {
          get {
            complete(getPerson(id).map(_.asJson))
          } ~
            delete {
              onSuccess(deletePerson(id)) { result =>
                complete(NoContent)
              }
            }
        } ~
          pathPrefix("followers") {
            pathEndOrSingleSlash {
              get {
                complete(getFollowers(id).runWith(Sink.seq).map(_.asJson))
              } ~
                post {
                  entity(as[Person]) { follower =>
                    onSuccess(addFollower(id, follower)) { result =>
                      complete(if (result) NoContent else BadRequest)
                    }
                  }
                }
            } ~
              pathPrefix(IntNumber) { followerId =>
                pathEndOrSingleSlash {
                  delete {
                    onSuccess(removeFollower(id, followerId)) { result =>
                      complete(if (result) NoContent else BadRequest)
                    }
                  }
                }

              }
          } ~
          pathPrefix("relation") {
            pathPrefix(IntNumber) { relationId =>
              pathEndOrSingleSlash {
                get {
                  complete(findRelation(id, relationId).map(_.asJson))
                }
              }
            }
          }
      }
  }
}
