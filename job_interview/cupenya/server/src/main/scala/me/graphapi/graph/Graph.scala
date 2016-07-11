package me.graphapi.graph

import akka.actor.{Actor, ActorRef, Props}
import akka.stream.ActorMaterializer
import akka.util.Timeout

import scala.concurrent.Future

final case class FindShortestPath(from: Int, to: Int, maxDepth: Int)

final case class ShortestPath(path: List[Int])

final case class GetVertex(id: Int)

abstract class Graph(materializer: ActorMaterializer, implicit val timeout: Timeout) extends Actor {

  implicit val executionContext = context.dispatcher

  val vertexProps: Props

  def receive = {
    case GetVertex(id) => handleGetVertex(id)
    case FindShortestPath(from, to, maxDepth) => handleFindShortestPath(from, to, maxDepth)
  }

  def hasVertex(id: Int): Future[Boolean]

  private def handleGetVertex(id: Int) {
    val replyTo = sender()

    hasVertex(id).map { hasVertex =>
      replyTo ! (if (hasVertex) Some(getOrCreateVertex(id.toString)) else None)
    }
  }

  private def getOrCreateVertex(name: String): ActorRef = {
    context.child(name).getOrElse(context.actorOf(vertexProps, name))
  }

  private def handleFindShortestPath(from: Int, to: Int, maxDepth: Int) {
    val replyTo = sender()
    val default = ShortestPath(List())

    doIfVertexExists(replyTo, from, default) {
      doIfVertexExists(replyTo, to, default) {
        val fromActor = getOrCreateVertex(from.toString)
        val aggregator = context.actorOf(Props(classOf[ShortestPathAggregator], replyTo, fromActor, to, maxDepth))
        fromActor ! FindPathTo(aggregator, List(), to)
      }
    }
  }

  private def doIfVertexExists[T](replyTo: ActorRef, id: Int, default: T)(f: => Unit) {
    hasVertex(id).map { hasVertex =>
      if (hasVertex)
        f
      else
        replyTo ! default
    }
  }
}
