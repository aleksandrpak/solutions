#Problem Statement

Destinations often have different type of accommodations and each one of them offers a different combination of facilities. Every traveler has a different set of requirements and constraints when preparing they trip and choosing where to stay. Some of them are business travelers, so they often look for more comfortable places to stay. Others travel with family and their ideal places offer some fun activities for the children.

Travelers ask you to help them to find the perfect accommodation!

#Input Format

The first line contains an integer N, which indicates the number of hotels that follow. Each hotel is represented by a single line of space-separated values, where the first value is the hotel ID (integer), the second value is its average price (integer) and the other values are the facilities offered by the hotel (space-separated strings). Each hotel has at least one facility. This is followed by another line containing an integer M, that indicates the number of test cases that follows. Each test case is represented by a single line of space-separated values, where the first value is the maximum budget for the guest (integer) and the rest forms a list of the guest’s required facilities (space separated strings). Each test case has at least one facility.

#Notes:

* All integers are unsigned integers with values between 0 and 2^32.
* All strings are 7-bit ASCII strings (this means they may contain no code above 127).
* Assume that N < 100, M <100

#Output Format

The output is precisely M lines, each one containing a space-separated list of hotel IDs. Each line will contain the IDs of the hotels that match up to the guest requirements, both in terms of price as well as facilities. The ID list should first be sorted in descending order by number of facilities. In case two hotels have the same number of facilities they should be sorted by price, starting with the cheapest. In case they have the same price, they should be sorted by ID starting with the smallest. If no hotel matches the requirements, a blank line should be produced.

##Sample Input

    4
    1 70 wifi pool restaurant bathtub kitchenette
    2 80 pool spa restaurant air-conditioning bathtub wifi
    3 60 restaurant air-conditioning wifi
    4 50 kitchenette
    4
    65 wifi
    50 wifi
    100 pool restaurant
    80 kitchenette

##Sample Output

    3

    2 1
    1 4
