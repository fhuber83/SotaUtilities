namespace PlantMaster2000
{
    public class PlantInfo
    {
        public string Name { get; set; } = "Unnamed";
        public int HoursToGrowInGreenhouse { get; set; } = 0;

        public PlantInfo()
        {

        }

        public PlantInfo(string name, int hoursToGrowInGreenhouse)
        {
            Name = name;
            HoursToGrowInGreenhouse = hoursToGrowInGreenhouse;
        }
    }
}
