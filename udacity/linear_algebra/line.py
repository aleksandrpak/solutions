from decimal import Decimal, getcontext
from vector import Vector

getcontext().prec = 30

class Line(object):

    NO_NONZERO_ELTS_FOUND_MSG = 'No nonzero elements found'

    def __init__(self, normal_vector=None, constant_term=None):
        self.dimension = 2

        if not normal_vector:
            all_zeros = ['0']*self.dimension
            normal_vector = Vector(all_zeros)
        self.normal_vector = normal_vector

        if not constant_term:
            constant_term = Decimal('0')
        self.constant_term = Decimal(constant_term)

        self.set_basepoint()


    def set_basepoint(self):
        try:
            n = self.normal_vector
            c = self.constant_term
            basepoint_coords = ['0']*self.dimension

            initial_index = Line.first_nonzero_index(n.coordinates)
            initial_coefficient = n.coordinates[initial_index]

            basepoint_coords[initial_index] = c/initial_coefficient
            self.basepoint = Vector(basepoint_coords)

        except Exception as e:
            if str(e) == Line.NO_NONZERO_ELTS_FOUND_MSG:
                self.basepoint = None
            else:
                raise e


    def __str__(self):

        num_decimal_places = 3

        def write_coefficient(coefficient, is_initial_term=False):
            coefficient = round(coefficient, num_decimal_places)
            if coefficient % 1 == 0:
                coefficient = int(coefficient)

            output = ''

            if coefficient < 0:
                output += '-'
            if coefficient > 0 and not is_initial_term:
                output += '+'

            if not is_initial_term:
                output += ' '

            if abs(coefficient) != 1:
                output += '{}'.format(abs(coefficient))

            return output

        n = self.normal_vector

        try:
            initial_index = Line.first_nonzero_index(n.coordinates)
            terms = [write_coefficient(n.coordinates[i], is_initial_term=(i==initial_index)) + 'x_{}'.format(i+1)
                     for i in range(self.dimension) if round(n.coordinates[i], num_decimal_places) != 0]
            output = ' '.join(terms)

        except Exception as e:
            if str(e) == self.NO_NONZERO_ELTS_FOUND_MSG:
                output = '0'
            else:
                raise e

        constant = round(self.constant_term, num_decimal_places)
        if constant % 1 == 0:
            constant = int(constant)
        output += ' = {}'.format(constant)

        return output


    @staticmethod
    def first_nonzero_index(iterable):
        for k, item in enumerate(iterable):
            if not MyDecimal(item).is_near_zero():
                return k
        raise Exception(Line.NO_NONZERO_ELTS_FOUND_MSG)

    def is_parallel_to(self, l):
        return self.normal_vector.is_parallel_to(l.normal_vector)

    def __eq__(self, l):
        if self.normal_vector.is_zero():
            if not l.normal_vector.is_zero():
                return False
            else:
                return MyDecimal(self.constant_term - l.constant_term).is_near_zero()
        elif l.normal_vector.is_zero():
            return False

        return self.is_parallel_to(l) and self.basepoint.minus(l.basepoint).is_orthogonal_to(self.normal_vector)

    def intersection_with(self, l):
        try:
            A, B = self.normal_vector.coordinates
            C, D = l.normal_vector.coordinates
            k1 = self.constant_term
            k2 = l.constant_term

            x_numerator = D * k1 - B * k2
            y_numerator = -C * k1 + A * k2
            denominator = Decimal('1') / (A * D - B * C)

            return Vector([x_numerator, y_numerator]).times_scalar(denominator)
        except:
            if self == l:
                return self
            else:
                return None

class MyDecimal(Decimal):
    def is_near_zero(self, eps=1e-10):
        return abs(self) < eps
