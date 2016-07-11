#!/usr/bin/env sh

echo "requesting: 1 - 6, expecting: empty"
curl http://localhost:8080/v1/persons/1/relation/6

echo "\n\nrequesting: 6 - 1, expecting: empty"
curl http://localhost:8080/v1/persons/6/relation/1

echo "\n\nrequesting: 1 - 16, expecting: empty"
curl http://localhost:8080/v1/persons/1/relation/16

echo "\n\nrequesting: 16 - 17, expecting: empty"
curl http://localhost:8080/v1/persons/16/relation/17

echo "\n\nrequesting: 1 - 4, expecting: 4 - 5 - 1"
curl http://localhost:8080/v1/persons/1/relation/4

echo "\n\nrequesting: 1 - 13, expecting: 13 - 12 - 11 - 10 - 1"
curl http://localhost:8080/v1/persons/1/relation/13

echo "\n\nrequesting: 8 - 9, expecting: 9 - 8"
curl http://localhost:8080/v1/persons/8/relation/9
