# python3
import heapq

class JobQueue:
    def read_data(self):
        self.num_workers, m = map(int, input().split())
        self.jobs = list(map(int, input().split()))
        assert m == len(self.jobs)

    def write_response(self):
        for i in range(len(self.jobs)):
          print(self.assigned_workers[i], self.start_times[i]) 

    def sift_down(a, i):
        l = 2 * i + 1
        r = 2 * i + 2

        m = i
        if l < len(a) and ((a[l][0] < a[m][0]) or (a[l][0] == a[m][0] and a[l][1] < a[m][1])):
            m = l

        if r < len(a) and ((a[r][0] < a[m][0]) or (a[r][0] == a[m][0] and a[r][1] < a[m][1])):
            m = r

        if i != m:
            a[i], a[m] = a[m], a[i]
            JobQueue.sift_down(a, m)

    def assign_jobs(self):
        # TODO: replace this code with a faster algorithm.
        self.assigned_workers = [None] * len(self.jobs)
        self.start_times = [None] * len(self.jobs)
        next_free_time = [(0, i) for i in range(self.num_workers)]

        for i in range(len(self.jobs)):
          self.assigned_workers[i] = next_free_time[0][1]
          self.start_times[i] = next_free_time[0][0]
          
          next_free_time[0] = (next_free_time[0][0] + self.jobs[i], next_free_time[0][1])
          JobQueue.sift_down(next_free_time, 0)

    def solve(self):
        self.read_data()
        self.assign_jobs()
        self.write_response()

if __name__ == '__main__':
    job_queue = JobQueue()
    job_queue.solve()

