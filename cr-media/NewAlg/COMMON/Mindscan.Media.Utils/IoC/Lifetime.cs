namespace Mindscan.Media.Utils.IoC
{
	public enum Lifetime
	{
		Transient = 0,
		Singleton,
		PerWebRequest,
		PerThread
	}
}