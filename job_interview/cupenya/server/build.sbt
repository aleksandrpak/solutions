name := "graphapi"

version := "1.0"

scalaVersion := "2.11.8"

libraryDependencies ++= {
  val akkaVersion = "2.4.6"
  val circeVersion = "0.4.1"

  Seq(
    "org.scala-lang" % "scala-reflect" % "2.11.8",

    "com.typesafe.akka" %% "akka-http-core" % akkaVersion,
    "com.typesafe.akka" %% "akka-http-experimental" % akkaVersion,

    "de.heikoseeberger" %% "akka-http-circe" % "1.6.0",

    "io.circe" %% "circe-core" % circeVersion,
    "io.circe" %% "circe-generic" % circeVersion
  )
}
