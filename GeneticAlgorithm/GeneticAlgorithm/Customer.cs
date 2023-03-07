namespace GeneticAlgorithm
{
    public class Customer
    {
        public readonly int RealId;
        public readonly int Id;
        public readonly (int, int) Coordinates;
        public int Demand;

        public Customer(int realId, (int, int) coordinates)
        {
            RealId = realId;
            Coordinates = coordinates;
            Demand = -1;
        }
        
        public Customer(int realId, (int, int) coordinates, int demand) : this(realId, coordinates)
        {
            Demand = demand;
        }
    }
}
