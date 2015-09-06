#Problem Statement

Booking.com offers more than 715,000 properties and we are constantly working to increase the number of partner businesses we work with. For that reason we came up with a self-build tool to allow partners to create their own “profile”, which translates into listing their property on the site. To make this process simple we’d like the partner to write down a description of the property and then we will use this to automatically prefill some of the required fields in the form.

The task here is to focus on extracting facilities that the property provides from the description written by the partner.

Taking the typed description and a list of the available facilities on our website, you need to find which set of facilities are offered at the property.

#Rules to extract Facilities:

* The facility should be exactly matching a part of the description; ex. Swimming pool is not matching Swimming-pool and not matching Swimming___pool
* No trailing spaces.
* Prefixes & Suffixes are ok; ex. if hotel description has 4Swimming pools, then this matches Swimming pool
* Matching is not case sensitive.

###Note: Property descriptions can be fairly long (up to 100,000 characters). There are fewer than 100 facilities on our list, and each named facility has fewer than 20 characters.

#Input Format

The first line is the number of available facilities; N (1 <= N < 100)

This is followed by N lines, each line is a facility.

The hotel description goes from line N+1 onwards.

#Output Format

The list of found facilities, one per line, this list of facilities is sorted alphabetically and as represented in the list of facilities.

###Note: It's case insensitive that's why the sample output wifi and WIFI are matching.

##Sample Input

```
5
wifi
swimming pool
garden
beach
tennis court
Our hotel is a very luxurious one, we are committed to provide the best holiday, you can always relax on our private beach, we provide four amazing tennis courts. WIFI is available in all areas.
```

##Sample Output

```
beach
tennis court
wifi
```