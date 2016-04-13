# python3
input()

vs = list(map(int, input().split()))
es = {}
r = -1

for v, p in enumerate(vs):
  if (p == -1):
    r = v
    continue

  if (p not in es):
    es[p] = [v]
  else:
    es[p].append(v)

st = [(r, 1)]
mh = 1

while (len(st) > 0):
  p, h = st.pop()

  if (p not in es):
    continue

  h += 1

  if (h > mh):
    mh = h

  for v in es[p]:
    st.append((v, h))

print(mh)
