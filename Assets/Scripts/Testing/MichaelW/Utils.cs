public class TypeUtils
{
	public static Direction FlipDirection(Direction dir)
	{
		return dir switch
		{
			Direction.Backward => Direction.Forward,
			Direction.Forward => Direction.Backward,
			_ => Direction.None,
		};
	}
}
