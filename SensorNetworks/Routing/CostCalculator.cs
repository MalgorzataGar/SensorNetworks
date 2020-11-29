namespace SensorNetworks
{
    public class CostCalculator
    {
        public double Calculate(int v_i, int v_j, AlgorithmParameters parameters)
        {
            parameters.SetLambda();
            return (parameters.Configuration.Beta / parameters.Configuration.Q) *
                   (parameters.D[v_i, v_j] / parameters.Delta[v_i, v_j]) *
                   (parameters.Configuration.E_max / (parameters.E_nergy[v_j] * parameters.Lambda[v_j])) *
                   (1 + (parameters.Configuration.Gamma * parameters.Tao[v_j]));
        }
    }
}