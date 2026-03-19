using UnityEngine;

namespace StepanoffGames.DiceRush.Math
{
	/// <summary>
	/// Математическая модель кривой Bezier 3го порядка.
	/// </summary>
	public class Bezier3
	{
		/// <summary>
		/// Закэшированное количество опорных точек.
		/// </summary>
		private int _pointsCount;
		/// <summary>
		/// Закэшированная величина аппроксимации.
		/// </summary>
		private int _approximation;
		/// <summary>
		/// Закэшированная полная длина кривой.
		/// </summary>
		private float _fullLength;

		/// <summary>
		/// Закэшированные координаты опорных точек.
		/// </summary>
		private float x0 = 0.0f;
		private float y0 = 0.0f;
		private float z0 = 0.0f;
		private float x1 = 0.0f;
		private float y1 = 0.0f;
		private float z1 = 0.0f;
		private float x2 = 0.0f;
		private float y2 = 0.0f;
		private float z2 = 0.0f;
		private float x3 = 0.0f;
		private float y3 = 0.0f;
		private float z3 = 0.0f;

		/// <summary>
		/// Опорные точки.
		/// </summary>
		private Vector3[] _points;

		public Bezier3(Vector3[] points, int approximation = 32)
		{
			SetPoints(points, approximation);
		}

		/// <summary>
		/// Утсановка опорных точек.
		/// <param name="points">координаты точек</param>
		/// </summary>
		public void SetPoints(Vector3[] points, int approximation = 32)
		{
			_points = points;
			_pointsCount = _points.Length;

			int count = 0;
			for (int i = 0; i < _points.Length; i++)
			{
				float x = _points[i].x;
				float y = _points[i].y;
				float z = _points[i].z;
				switch (i)
				{
					case 0: x0 = x; y0 = y; z0 = z; break;
					case 1: x1 = x; y1 = y; z1 = z; break;
					case 2: x2 = x; y2 = y; z2 = z; break;
					case 3: x3 = x; y3 = y; z3 = z; break;
				}
				count++;
			}

			//CalcApproximation();

			_approximation = approximation;
			_fullLength = GetLength(0.0f, 1.0f);
		}

		/// <summary>
		/// Величина аппроксимации.
		/// </summary>
		public int Approximation
		{
			get
			{
				return _approximation;
			}
			set
			{
				if (_approximation != value)
				{
					_approximation = value;
					_fullLength = GetLength(0.0f, 1.0f);
				}
			}
		}

		/// <summary>
		/// Количество опорных точек.
		/// </summary>
		public int pointsCount
		{
			get
			{
				return _pointsCount;
			}
		}

		/// <summary>
		/// Полная длина кривой.
		/// </summary>
		public float fullLength
		{
			get
			{
				return _fullLength;
			}
		}

		/// <summary>
		/// Возвращает опорную точку по индексу.
		/// <param name="index">индекс опорной точки</param>
		/// </summary>
		public Vector3 GetPoint(int index)
		{
			Vector3 p = Vector3.zero;
			if ((index >= 0) && (index <= (_pointsCount - 1)))
			{
				p = _points[index];
			}
			return p;
		}

		/// <summary>
		/// Точность вычислений длины линии, итератора линии
		/// </summary>
		private const float TOLERANCE = 0.001f;//0.0001f;

		/// <summary>
		/// Минимальное количество шагов аппроксимации линии; используется функцией GetApproximation()
		/// </summary>
		private const int MIN_APPROXIMATION = 20;
		/// <summary>
		/// Максимальное количество шагов аппроксимации линии; используется функцией GetApproximation()
		/// </summary>
		private const int MAX_APPROXIMATION = 100;

		/// <summary>
		/// Вычисляет адаптивное количество шагов аппроксимации в зависимости от длины и искривленности линии.
		/// Количество шагов лежит в пределах, задаваемых константами MIN_APPROXIMATION и MAX_APPROXIMATION.
		/// </summary>
		//public void CalcApproximation()
		//{
		//	float maxLen = 0.0f;
		//	maxLen += Vector3.Distance(points[0], points[1]);
		//	if (_pointsCount == 4)
		//	{
		//		maxLen += Vector3.Distance(points[1], points[2]);
		//		maxLen += Vector3.Distance(points[2], points[3]);
		//	}
		//	float minLen = 0.0f;
		//	if (_pointsCount == 2)
		//	{
		//		minLen = Vector3.Distance(points[0], points[1]);
		//	}
		//	else
		//	{
		//		minLen = Vector3.Distance(points[0], points[3]);
		//	}

		//	int n = 1;
		//	if (minLen != 0)
		//	{
		//		n = (int)Mathf.Round((maxLen / 40.0f) * ((maxLen / minLen - 1.0f) / 4.0f + 1.0f));
		//		if (n < MIN_APPROXIMATION) n = MIN_APPROXIMATION;
		//		else if (n > MAX_APPROXIMATION) n = MAX_APPROXIMATION;
		//	}
		//	_approximation = n;
		//}

		/// <summary>
		/// Возвращает итератор линии, соответствующий указанному расстоянию len от первой опорной точки линии.
		/// Если расстояние len отрицательное, то возвращаемое значение будет отрицательное;
		/// если len больше или равно 0 и меньше или равно длине линии - возвращаемое значение итератора будет в диапазоне 0 <= t <= 1;
		/// если len больше длины линии - возвращаемое значение итератора будет больше 1.
		/// <param name="len">расстояние</param>
		/// </summary>
		public float GetIteratorByLength(float len)
		{
			float t = 0.0f;
			// если линия - прямая
			if (_pointsCount == 2)
			{
				t = len / Vector3.Distance(GetPointByIterator(0.0f), GetPointByIterator(1.0f));
			}
			// если линия - кривая Безье
			else if ((_pointsCount == 3) || (_pointsCount == 4))
			{
				float l = 0.0f;                  // length - расстояние до точки
				float d = 1.0f / _approximation; // delta - шаг аппроксимации
				int s = 1;                       // sign - направление приращения расстояния
				int n = 0;
				while ((Mathf.Abs(len - l) > TOLERANCE) && (n < 100))
				{
					n++;
					int m = 0;
					while ((((s == 1) && (l < len)) || ((s == -1) && (l > len))) && (m < 100))
					{
						m++;
						l += s * GetArcLength(GetPointByIterator(t),
							GetPointByIterator(t + s * d / 2.0f),
							GetPointByIterator(t + s * d));
						t += s * d;
					}
					s = -s;
					d = d / 2.0f;
				}
			}
			return t;
		}

		/// <summary>
		/// Возвращает точку на линии, соответствующую указанному итератору t.
		/// Если t < 0, то возвращаемая точка будет лежать на касательной, проведенной через первую опорную точку линии;
		/// если 0 <= t <= 1 - возвращаемая точка будет лежать на линии;
		/// если t > 1 - возвращаемая точка будет лежать на касательной, проведенной через последнюю опорную точку линии.
		/// <param name="t">итератор</param>
		/// </summary>
		public Vector3 GetPointByIterator(float t)
		{
			float x = 0.0f;
			float y = 0.0f;
			float z = 0.0f;

			// если итератор лежит за пределами области построения линии,
			// возвращает точку на касательной, построенной на соответствующей конечной точке линии
			if ((t < 0.0) || (t > 1.0))
			{
				Vector3 cp = Vector3.zero;
				Vector3 dp = Vector3.zero;
				if (t < 0.0)
				{
					cp = GetPointByIterator(0.0f);
					dp = GetDerivative(0.0f);
				}
				else
				{
					cp = GetPointByIterator(1.0f);
					dp = GetDerivative(1.0f);
					t -= 1.0f;
				}
				x = cp.x + dp.x * t;
				y = cp.y + dp.y * t;
				z = cp.z + dp.z * t;
			}
			// если итератор лежит в пределах области построения линии,
			// возвращает точку, соответствующую уравнению линии
			else
			{
				// прямая
				if (_pointsCount == 2)
				{
					x = (1.0f - t) * x0 + t * x1;
					y = (1.0f - t) * y0 + t * y1;
					z = (1.0f - t) * z0 + t * z1;
				}
				// кривая Безье 3-го порядка
				else if (_pointsCount == 4)
				{
					/*float t2 = t * t;
					float t3 = t2 * t;

					float k0 = 1.0f - 3.0f * t + 3.0f * t2 - t3;
					float k1 = 3.0f * t - 6.0f * t2 + 3.0f * t3;
					float k2 = 3.0f * t2 - 3.0f * t3;
					float k3 = t3;

					x = k0 * x0 + k1 * x1 + k2 * x2 + k3 * x3;
					y = k0 * y0 + k1 * y1 + k2 * y2 + k3 * y3;*/

					float u = 1.0f - t;
					float tt = t * t;
					float uu = u * u;
					float uuu = uu * u;
					float ttt = tt * t;

					Vector3 p = uuu * _points[0]; // first term
					p += 3 * uu * t * _points[1]; // second term
					p += 3 * u * tt * _points[2]; // third term
					p += ttt * _points[3];        // fourth term

					x = p.x;
					y = p.y;
					z = p.z;
				}
			}

			return new Vector3(x, y, z);
		}

		/// <summary>
		/// Возвращает точку на линии, соответствующую указанному расстоянию len от первой опорной точки линии.
		/// Если расстояние len отрицательное, то возвращаемая точка будет лежать на касательной, проведенной через первую опорную точку линии;
		/// если len больше или равно 0 и меньше или равно длине линии - возвращаемая точка будет лежать на линии;
		/// если len больше длины линии - возвращаемая точка будет лежать на касательной, проведенной через последнюю опорную точку линии.
		/// <param name="len">расстояние</param>
		/// </summary>
		public Vector3 GetPointByLength(float len)
		{
			return GetPointByIterator(GetIteratorByLength(len));
		}

		/// <summary>
		/// Возвращает производную в точке, определенной итератором t.
		/// Если t < 0, то производная вычисляется для t = 0 (что соответствует касательной, проведенной через первую опорную точку линии);
		/// если t > 1 - производная вычисляется для t = 1 (что соответствует касательной, проведенной через последнюю опорную точку линии).
		/// <param name="t">итератор линии</param>
		/// </summary>
		public Vector3 GetDerivative(float t)
		{
			float x = 0.0f;
			float y = 0.0f;
			float z = 0.0f;

			if (_pointsCount == 2)
			{
				x = -x0 + x1;
				y = -y0 + y1;
				z = -y0 + y1;
			}
			else if (_pointsCount == 4)
			{
				/*float t2 = t * t;

				float k0 = -3.0 + 6.0 * t - 3.0 * t2;
				float k1 = 3.0 - 12.0 * t + 9.0 * t2;
				float k2 = 6.0 * t - 9.0 * t2;
				float k3 = 3.0 * t2;

				x = k0 * x0 + k1 * x1 + k2 * x2 + k3 * x3;
				y = k0 * y0 + k1 * y1 + k2 * y2 + k3 * y3;*/

				float u = -1.0f;
				float tt = 2.0f * t;
				float uu = 2.0f * u;
				float uuu = 3.0f * u * u;
				float ttt = 3.0f * t * t;

				Vector3 p = uuu * _points[0]; // first term
				p += 3 * uu * t * _points[1]; // second term
				p += 3 * u * tt * _points[2]; // third term
				p += ttt * _points[3];        // fourth term

				x = p.x;
				y = p.y;
				z = p.z;
			}

			return new Vector3(x, y, z);
		}

		/// <summary>
		/// Возвращает длину линии от значения итератора t1 до значения итератора t2.
		/// <param name="t1">итератор линии - начало диапазона вычисления длины</param>
		/// <param name="t2">итератор линии - конец диапазона вычисления длины</param>
		/// </summary>
		public float GetLength(float t1, float t2)
		{
			float l = 0.0f;
			if (_pointsCount == 2)
			{
				l = Vector3.Distance(GetPointByIterator(t1), GetPointByIterator(t2));
			}
			else if ((_pointsCount == 3) || (_pointsCount == 4))
			{
				float t = t1;
				float d = (t2 - t1) / _approximation;
				for (int i = 0; i < _approximation; i++)
				{
					l += GetArcLength(GetPointByIterator(t),
						GetPointByIterator(t + d / 2.0f),
						GetPointByIterator(t + d));
					t += d;
				}
			}
			return l;
		}

		/// <summary>
		/// Возвращает итератор, соответствующий точке на линии, ближайшей к произвольной точке p;
		/// линия исследуется в интервале от значения итератора 0 до значения итератора 1.
		/// <param name="p">произвольная точка</param>
		/// </summary>
		public float GetNearestIterator(Vector3 p)
		{
			return GetNearestIterator(p, 0.0f, 1.0f);
		}

		/// <summary>
		/// Возвращает итератор, соответствующий точке на линии, ближайшей к произвольной точке p;
		/// линия исследуется в интервале от значения итератора t1 до значения итератора t2.
		/// <param name="p">произвольная точка</param>
		/// <param name="t1">итератор линии - начало диапазона исследования</param>
		/// <param name="t2">итератор линии - конец диапазона исследования</param>
		/// </summary>
		public float GetNearestIterator(Vector3 p, float t1, float t2)
		{
			// с шагом 1/approximation находим минимальное расстояние от указанной произвольной точки до линии
			float minLen = Vector3.Distance(GetPointByIterator(t1), p);
			float minLenT = 0.0f;

			float len = 0.0f;
			float dt = 1.0f / _approximation;
			float t = t1;
			int n = 0;
			while ((t < t2) && (n < 100))
			{
				t += dt;
				len = Vector3.Distance(GetPointByIterator(t), p);
				if ((len < minLen))
				{
					minLen = len;
					minLenT = t;
				}
			}

			// minLenT - итератор, соответствующий точке линии, в которой расстояние до указанной произвольной точки минимальное
			// уменьшая шаг, ищем в окрестностях данного итератора уточненное значение
			t = minLenT;
			dt /= (t >= t2) ? -2.0f : 2.0f;
			len = minLen;
			n = 0;
			do
			{
				n++;
				int m = 0;
				do
				{
					m++;
					t += dt;
					minLen = len;
					len = Vector3.Distance(GetPointByIterator(t), p);
				} while ((len < minLen) && (t >= t1) && (t <= t2) && (m < 100));
				dt /= -2.0f;
			} while ((Mathf.Abs(len - minLen) > TOLERANCE) && (n < 100));

			if (t < t1) t = t1;
			else if (t > t2) t = t2;
			return t;
		}

		/// <summary>
		/// Возвращает точку на линии, ближайшую к произвольной точке p;
		/// линия исследуется в интервале от значения итератора 0 до значения итератора 1.
		/// <param name="p">произвольная точка</param>
		/// </summary>
		public Vector3 GetNearestPoint(Vector3 p)
		{
			return GetNearestPoint(p, 0.0f, 1.0f);
		}

		/// <summary>
		/// Возвращает точку на линии, ближайшую к произвольной точке p;
		/// линия исследуется в интервале от значения итератора t1 до значения итератора t2.
		/// <param name="p">произвольная точка</param>
		/// <param name="t1">итератор линии - начало диапазона исследования</param>
		/// <param name="t2">итератор линии - конец диапазона исследования</param>
		/// </summary>
		public Vector3 GetNearestPoint(Vector3 p, float t1, float t2)
		{
			return GetPointByIterator(GetNearestIterator(p, t1, t2));
		}

		/// <summary>
		/// Возвращает минимальное расстояние от произвольной точки p до линии;
		/// линия исследуется в интервале от значения итератора 0 до значения итератора 1.
		/// <param name="p">произвольная точка</param>
		/// </summary>
		public float GetDistance(Vector3 p)
		{
			return GetDistance(p, 0.0f, 1.0f);
		}

		/// <summary>
		/// Возвращает минимальное расстояние от произвольной точки p до линии;
		/// линия исследуется в интервале от значения итератора t1 до значения итератора t2.
		/// <param name="p">произвольная точка</param>
		/// <param name="t1">итератор линии - начало диапазона исследования</param>
		/// <param name="t2">итератор линии - конец диапазона исследования</param>
		/// </summary>
		public float GetDistance(Vector3 p, float t1, float t2)
		{
			return Vector3.Distance(GetNearestPoint(p, t1, t2), p);
		}

		/// <summary>
		/// Возвращает длину дуги окружности, определенной точками p1, p2, p3.
		/// <param name="p1">начальная точка дуги</param>
		/// <param name="p2">сентральная точка дуги</param>
		/// <param name="p3">конечная точка дуги</param>
		/// </summary>
		protected float GetArcLength(Vector3 p1, Vector3 p2, Vector3 p3)
		{
			return Vector3.Distance(p1, p2) + Vector3.Distance(p2, p3);
		}
	}
}
