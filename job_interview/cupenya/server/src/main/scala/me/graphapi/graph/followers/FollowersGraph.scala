package me.graphapi.graph.followers

import akka.actor.Props
import akka.stream.ActorMaterializer
import akka.util.Timeout
import me.graphapi.graph.Graph
import me.graphapi.util.DatabaseService

import scala.concurrent.Future

class FollowersGraph(databaseService: DatabaseService, materializer: ActorMaterializer,
                     timeout: Timeout) extends Graph(materializer, timeout) {

  val vertexProps = Props(classOf[FollowerVertex], databaseService, materializer, timeout)

  def hasVertex(id: Int): Future[Boolean] =databaseService.hasPerson(id)
}
