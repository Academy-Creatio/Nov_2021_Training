namespace Training.Interfaces
{

	/// <summary>
	/// MAKE SURE  TO PROVIDE USERCONNECTION IN CONSTRUCTORARGS
	/// </summary>
	public interface ICalculator
	{
		/// <summary>
		/// Adds two ins together
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns>result of operation</returns>
		int Add(int a, int b);
		int Sub(int a, int b);
		int Multiply(int a, int b);

	}
}