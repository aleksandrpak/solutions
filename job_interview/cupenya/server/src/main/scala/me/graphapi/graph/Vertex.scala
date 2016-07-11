package me.graphapi.graph

import akka.NotUsed
import akka.actor.{Actor, ActorRef}
import akka.pattern._
import akka.stream.ActorMaterializer
import akka.stream.scaladsl.Source
import akka.util.Timeout

import scala.concurrent.Future

final case class FindPathTo(replyTo: ActorRef, path: List[Int], to: Int, targetDepth: Int = 1, currentDepth: Int = 1)

final case class PathTo(path: List[Int], targetDepth: Int)

abstract class Vertex(implicit val materializer: ActorMaterializer, implicit val timeout: Timeout) extends Actor {

  implicit val executionContext = context.dispatcher

  val id = self.path.name.toInt

  def hasEdgeTo(to: Int): Future[Boolean]

  def getEdges: Source[Int, NotUsed]

  def receive = {
    case FindPathTo(replyTo, path, to, targetDepth, currentDepth) =>
      if (currentDepth == targetDepth) {
        hasEdgeTo(to).map { hasEdge =>
          replyTo ! PathTo(if (hasEdge) to :: id :: path else List(), targetDepth)
        }
      } else {
        val findPathToRequest = FindPathTo(replyTo, id :: path, to, targetDepth, currentDepth + 1)

        getEdges.runFold(0) { (counter, vertexId) =>
          if (counter == 0) {
            findPathTo(vertexId, findPathToRequest)
          } else {
            (replyTo ? IncrementExpectedResponses).foreach { result =>
              findPathTo(vertexId, findPathToRequest)
            }
          }

          counter + 1
        }.foreach { counter =>
          if (counter == 0)
            replyTo ! PathTo(List(), targetDepth)
        }
      }
  }

  def findPathTo(vertexId: Int, findPathToRequest: FindPathTo): Unit = {
    (context.parent ? GetVertex(vertexId)).foreach {
      case Some(follower: ActorRef) => follower ! findPathToRequest
    }
  }
}
