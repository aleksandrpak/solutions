#Problem Statement

Lots of users are visiting Booking.com trying to plan a trip to visit multiple cities, usually those user have a budget on how much they can spend on their trip. We wanna help those users to find the best hotels which is fitting within their budget.

You will get the list of cities a user is interested to visit, also a list of hotels within each city, each hotel has a price and a score which, and of course we would have the amount of euros that the user is ready to spend.

You are required to find the highest total score of the hotels that the user can book within his budget.

#Input Format

First line has two numbers number of cities C and user's budget B, each city will start with a number N which is the number of available hotels in this city, followed by N line, each line has two numbers, an integer representing hotel price (Pij) & a number representing hotel score (Sij).

```
0 < C <=10
0 < N <= 100
0 < B <= 10000
0 < Pij <= 10000
0 < Sij <= 10
```

#Output Format

One number which is the highest total score for the hotels where the user can book, -1 if the user budget can't achieve booking the whole trip.

Print two digits after decimal point, rounded to 2 decimal places. If the answer is -1, than just print "-1".

###Note: User has to book exactly one hotel per destination, and he has to book all destinations

##Sample Input

```
2 50
3
10 7.8
15 6.4
12 8.111
3
25 7.8
19 6.4
50 8.1
```

##Sample Output

```
15.91
```