from math import sqrt, acos, pi
from decimal import Decimal, getcontext

getcontext().prec = 30

class Vector(object):
    CANNOT_NORMALIZE_ZERO_VECTOR_MSG = 'Cannot normalize the zero vector'
    NO_UNIQUE_ORTHOGONAL_COMPONENT_MSG = 'No unique orthogonal component'
    NO_UNIQUE_PARALLEL_COMPONENT_MSG = 'No unique parallel component'
    ONLY_DEFINED_IN_TWO_THREE_DIMS_MSG = 'Cross function is only defined for 2d and 3d'

    def __init__(self, coordinates):
        try:
            if not coordinates:
                raise ValueError
            self.coordinates = tuple([Decimal(x) for x in coordinates])
            self.dimension = len(coordinates)

        except ValueError:
            raise ValueError('The coordinates must be nonempty')

        except TypeError:
            raise TypeError('The coordinates must be an iterable')


    def __str__(self):
        return 'Vector: {}'.format(self.coordinates)


    def __eq__(self, v):
        return self.coordinates == v.coordinates

    def plus(self, v):
        return Vector([x+y for x,y in zip(self.coordinates, v.coordinates)])

    def minus(self, v):
        return Vector([x-y for x,y in zip(self.coordinates, v.coordinates)])

    def times_scalar(self, c):
        return Vector([Decimal(c)*x for x in self.coordinates])

    def magnitude(self):
        return sqrt(sum([x**2 for x in self.coordinates]))

    def normalized(self):
        try:
            return self.times_scalar(1./self.magnitude())
        except ZeroDivisionError:
            raise Exception(CANNOT_NORMALIZE_ZERO_VECTOR_MSG)

    def dot(self, v):
        return sum([x*y for x,y in zip(self.coordinates, v.coordinates)])

    def angle_with(self, v, in_degrees=False):
        try:
            n1 = self.normalized()
            n2 = v.normalized()
            d = n1.dot(n2)
            d = min(d, Decimal('1'))
            d = max(d, Decimal('0'))
            angle_in_radians = acos(d)

            if in_degrees:
                degrees_per_radian = 180. / pi
                return angle_in_radians * degrees_per_radian
            else:
                return angle_in_radians
        except Exception as e:
            if str(e) == self.CANNOT_NORMALIZE_ZERO_VECTOR_MSG:
                raise Exception('Cannot compute an angle with the zero vector')
            else:
                raise e

    def is_orthogonal_to(self, v, tolerance=1e-10):
        return abs(self.dot(v)) < tolerance

    def is_parallel_to(self, v):
        if self.is_zero() or v.is_zero():
            return True

        angle = self.angle_with(v)
        return angle == 0 or angle == pi

    def is_zero(self, tolerance=1e-10):
        return self.magnitude() < tolerance

    def component_orthogonal_to(self, basis):
        try:
            return self.minus(self.component_parallel_to(basis))
        except Exception as e:
            if str(e) == self.NO_UNIQUE_PARALLEL_COMPONENT_MSG:
                raise Exception(self.NO_UNIQUE_ORTHOGONAL_COMPONENT_MSG)
            else:
                raise e

    def component_parallel_to(self, basis):
        try:
            u = basis.normalized()
            weight = self.dot(u)
            return u.times_scalar(weight)
        except Exception as e:
            if str(e) == self.CANNOT_NORMALIZE_ZERO_VECTOR_MSG:
                raise Exception(self.NO_UNIQUE_PARALLEL_COMPONENT_MSG)
            else:
                raise e

    def cross(self, v):
        try:
            x_1, y_1, z_1 = self.coordinates
            x_2, y_2, z_2 = v.coordinates

            return Vector([y_1 * z_2 - y_2 * z_1, -(x_1 * z_2 - x_2 * z_1), x_1 * y_2 - x2_2 * y1])
        except ValueError as e:
            msg = str(e)
            if msg == 'need more than 2 values to unpack':
                self_embedded_in_R3 = Vector(self.coordinates + ('0',))
                v_embedded_in_R3 = Vector(v.coordinates + ('0',))
                return self_embedded_in_R3.cross(v_embedded_in_R3)
            elif msg == 'too many values to unpack' or msg == 'need more than value to unpack':
                raise Exception(self.ONLY_DEFINED_IN_TWO_THREE_DIMS_MSG)
            else:
                raise e

    def area_of_triangle_with(self, v):
        return self.area_of_parallelogram_with(v) / Decimal('2.0')

    def area_of_parallelogram_with(self, v):
        return self.cross(v).magnitude()
