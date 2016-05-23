package me.graphapi.util

import com.typesafe.config.ConfigFactory

trait Config {
  private val config = ConfigFactory.load()
  private val graphConfig = config.getConfig("graph")
  private val httpConfig = config.getConfig("http")

  val shortestPathMaxDepth = graphConfig.getInt("shortestPathMaxDepth")
  val operationsTimeout = graphConfig.getInt("operationsTimeout")

  val httpHost = httpConfig.getString("interface")
  val httpPort = httpConfig.getInt("port")
}
