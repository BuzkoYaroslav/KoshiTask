using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace library
{
    static class AdamsMethod
    {
        private const int n = 0;
        private const int maxRightBound = 100;
        private const double funcConditionStep = 0.0001;
        private const double eps = 0.000001;

        public static KeyValuePair<double, double>[] Solve(KoshiTask task, double step)
        {
            List<KeyValuePair<double, double>> result = new List<KeyValuePair<double, double>>();

            KoshiTask supportTask = new KoshiTask(task.Derivative, task.StartCondition, 
                new KeyValuePair<double, double>(task.Range.Key, task.Range.Key + step * n));
            result.AddRange(RungeKuttaMethod.Solve(supportTask, n));

            double[] A = new double[n + 2];
            for (int i = 0; i < n + 2; i++)
                A[i] = Ak(i - 1);

            int index = n;

            while (result[index].Key + step <= task.Range.Value)
            {
                Dictionary<uint, double> vars = new Dictionary<uint, double>();
                vars.Add(0, 1);
                vars.Add(1, 1);

                double sum = result[index].Value;
                for (int i = 1; i < n + 2; i++)
                {
                    vars[0] = result[index + 1 - i].Key;
                    vars[1] = result[index + 1 - i].Value;
                    sum += step * task.Derivative.Calculate(vars);
                }

                double yprev, 
                    ynext = sum;

                vars.Remove(1);
                vars[0] = result[index].Key + step;

                MathFunction func = sum + step * A[0] * task.Derivative.TransformToSimpleFunction(vars);

                do
                {
                    yprev = ynext;
                    ynext = func.Calculate(sum);
                } while (Math.Abs(yprev - ynext) > eps);

                result.Add(new KeyValuePair<double, double>(vars[0], ynext));
                index++;
            }

            return result.ToArray();
        }

        
        private static KeyValuePair<double, double> NewtonBoundaries(double startLeft, double difference, MathFunction func)
        {
            MathFunction func1 = func.Derivative(1),
                    func2 = func.Derivative(2);

            Func<double, MathFunction, bool, bool> condition = (x, function, isPositive) =>
            {
                double value = function.Calculate(x);

                return !double.IsInfinity(value) && !double.IsNaN(value) &&
                       (isPositive && value >= 0 || !isPositive && value <= 0);
            };

            bool isPositive1 = func1.Calculate(startLeft) > 0,
                 isPositive2 = func2.Calculate(startLeft) > 0;

            double right = difference, left = -difference;

            for (double i = startLeft; i < difference; i += funcConditionStep)
                if (!(condition(i, func1, isPositive1) && condition(i, func2, isPositive2)))
                {
                    right = i - funcConditionStep;
                    break;
                }

            for (double i = startLeft; i > -difference; i -= funcConditionStep)
                if (!(condition(i, func1, isPositive1) && condition(i, func2, isPositive2)))
                {
                    left = i + funcConditionStep;
                    break;
                }

            return new KeyValuePair<double, double>(left, right);
        }
        private static double Ak(int k)
        {
            MathFunction res = 1;

            for (int i = -1; i <= n; i++)
                if (i != k)
                    res *= new XFunction(1.0) + i;

            double result = new SimpsonMethod().Solve(res, 0, 1, 10);

            Func<long, long> Factorial = null;
            Factorial = x => x == 0 ? 1 : x * Factorial(x - 1);

            result *= Math.Pow(-1, k + 1) / (Factorial(k + 1) * Factorial(n - k));

            return result;
        }
    }
}
