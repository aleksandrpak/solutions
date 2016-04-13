# python3

class Request:
    def __init__(self, arrival_time, process_time):
        self.arrival_time = arrival_time
        self.process_time = process_time

class Response:
    def __init__(self, start_time):
        self.start_time = start_time

class Buffer:
    def __init__(self, size):
        self.size = size
        self.finish_time = []

    def Process(self, request):
        while (len(self.finish_time) > 0 and self.finish_time[-1] <= request.arrival_time):
            self.finish_time.pop()

        if (self.size == len(self.finish_time)):
            return Response(-1)

        last = request.arrival_time
        if (len(self.finish_time) > 0):
            last = self.finish_time[0]

        self.finish_time.insert(0, last + request.process_time)

        return Response(last)

def ReadRequests(count):
    requests = []
    for i in range(count):
        arrival_time, process_time = map(int, input().strip().split())
        requests.append(Request(arrival_time, process_time))
    return requests

def ProcessRequests(requests, buffer):
    responses = []
    for request in requests:
        responses.append(buffer.Process(request))
    return responses

def PrintResponses(responses):
    for response in responses:
        print(response.start_time)

if __name__ == "__main__":
    size, count = map(int, input().strip().split())
    requests = ReadRequests(count)

    buffer = Buffer(size)
    responses = ProcessRequests(requests, buffer)

    PrintResponses(responses)
