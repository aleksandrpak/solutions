#!/usr/bin/env sh

curl -H "Content-Type: application/json" -X POST -d '{"firstName":"first1","lastName":"last1"}'   http://localhost:8080/v1/persons &>/dev/null
curl -H "Content-Type: application/json" -X POST -d '{"firstName":"first2","lastName":"last2"}'   http://localhost:8080/v1/persons &>/dev/null
curl -H "Content-Type: application/json" -X POST -d '{"firstName":"first3","lastName":"last3"}'   http://localhost:8080/v1/persons &>/dev/null
curl -H "Content-Type: application/json" -X POST -d '{"firstName":"first4","lastName":"last4"}'   http://localhost:8080/v1/persons &>/dev/null
curl -H "Content-Type: application/json" -X POST -d '{"firstName":"first5","lastName":"last5"}'   http://localhost:8080/v1/persons &>/dev/null
curl -H "Content-Type: application/json" -X POST -d '{"firstName":"first6","lastName":"last6"}'   http://localhost:8080/v1/persons &>/dev/null
curl -H "Content-Type: application/json" -X POST -d '{"firstName":"first7","lastName":"last7"}'   http://localhost:8080/v1/persons &>/dev/null
curl -H "Content-Type: application/json" -X POST -d '{"firstName":"first8","lastName":"last8"}'   http://localhost:8080/v1/persons &>/dev/null
curl -H "Content-Type: application/json" -X POST -d '{"firstName":"first9","lastName":"last9"}'   http://localhost:8080/v1/persons &>/dev/null
curl -H "Content-Type: application/json" -X POST -d '{"firstName":"first10","lastName":"last10"}' http://localhost:8080/v1/persons &>/dev/null
curl -H "Content-Type: application/json" -X POST -d '{"firstName":"first11","lastName":"last11"}' http://localhost:8080/v1/persons &>/dev/null
curl -H "Content-Type: application/json" -X POST -d '{"firstName":"first12","lastName":"last12"}' http://localhost:8080/v1/persons &>/dev/null
curl -H "Content-Type: application/json" -X POST -d '{"firstName":"first13","lastName":"last13"}' http://localhost:8080/v1/persons &>/dev/null
curl -H "Content-Type: application/json" -X POST -d '{"firstName":"first14","lastName":"last14"}' http://localhost:8080/v1/persons &>/dev/null
curl -H "Content-Type: application/json" -X POST -d '{"firstName":"first15","lastName":"last15"}' http://localhost:8080/v1/persons &>/dev/null

curl -H "Content-Type: application/json" -X POST -d '{"id":2}'  http://localhost:8080/v1/persons/1/followers
curl -H "Content-Type: application/json" -X POST -d '{"id":5}'  http://localhost:8080/v1/persons/1/followers
curl -H "Content-Type: application/json" -X POST -d '{"id":7}'  http://localhost:8080/v1/persons/1/followers
curl -H "Content-Type: application/json" -X POST -d '{"id":10}' http://localhost:8080/v1/persons/1/followers
curl -H "Content-Type: application/json" -X POST -d '{"id":14}' http://localhost:8080/v1/persons/1/followers
curl -H "Content-Type: application/json" -X POST -d '{"id":3}'  http://localhost:8080/v1/persons/2/followers
curl -H "Content-Type: application/json" -X POST -d '{"id":4}'  http://localhost:8080/v1/persons/3/followers
curl -H "Content-Type: application/json" -X POST -d '{"id":4}'  http://localhost:8080/v1/persons/5/followers
curl -H "Content-Type: application/json" -X POST -d '{"id":8}'  http://localhost:8080/v1/persons/7/followers
curl -H "Content-Type: application/json" -X POST -d '{"id":9}'  http://localhost:8080/v1/persons/8/followers
curl -H "Content-Type: application/json" -X POST -d '{"id":11}' http://localhost:8080/v1/persons/10/followers
curl -H "Content-Type: application/json" -X POST -d '{"id":12}' http://localhost:8080/v1/persons/11/followers
curl -H "Content-Type: application/json" -X POST -d '{"id":13}' http://localhost:8080/v1/persons/12/followers
curl -H "Content-Type: application/json" -X POST -d '{"id":15}' http://localhost:8080/v1/persons/14/followers
curl -H "Content-Type: application/json" -X POST -d '{"id":1}'  http://localhost:8080/v1/persons/15/followers
curl -H "Content-Type: application/json" -X POST -d '{"id":14}' http://localhost:8080/v1/persons/15/followers

