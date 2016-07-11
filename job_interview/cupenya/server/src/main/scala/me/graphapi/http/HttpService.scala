package me.graphapi.http

import akka.http.scaladsl.server.Directives._
import akka.stream.ActorMaterializer
import me.graphapi.http.routes.PersonsServiceRouter
import me.graphapi.services.PersonsService

import scala.concurrent.ExecutionContext

class HttpService(personsService: PersonsService)(implicit val executionContext: ExecutionContext,
                                                  implicit val materializer: ActorMaterializer) {

  val personsRouter = new PersonsServiceRouter(personsService)

  val routes =
    pathPrefix("v1") {
      personsRouter.route
    }
}
