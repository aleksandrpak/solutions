from vector import Vector
from line import Line

l1 = Line(normal_vector=Vector(['4.046', '2.836']), constant_term='1.21')
l2 = Line(normal_vector=Vector(['10.115', '7.09']), constant_term='3.025')

print(l1.intersection_with(l2))

l1 = Line(normal_vector=Vector(['7.204', '3.182']), constant_term='8.68')
l2 = Line(normal_vector=Vector(['8.172', '4.114']), constant_term='9.883')

print(l1.intersection_with(l2))

l1 = Line(normal_vector=Vector(['1.182', '5.562']), constant_term='6.744')
l2 = Line(normal_vector=Vector(['1.773', '8.343']), constant_term='9.525')

print(l1.intersection_with(l2))
