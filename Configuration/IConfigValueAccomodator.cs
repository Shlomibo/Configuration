using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Configuration
{
	public interface IConfigValueAccomodator
	{
		object Accomodate(object value);
		object AccomodateBack(object value);
	}

	public interface IConfigValueAccomodator<TIn, TOut> : IConfigValueAccomodator
	{
		TOut Accomodate(TIn value);
		TIn AccomodateBack(TOut value);
	}

	public abstract class ConfigValueAccomodator<TIn, TOut> : IConfigValueAccomodator<TIn, TOut>
	{
		public abstract TOut Accomodate(TIn value);

		public abstract TIn AccomodateBack(TOut value);

		public object Accomodate(object value)
		{
			return Accomodate((TIn)value);
		}

		public object AccomodateBack(object value)
		{
			return AccomodateBack((TOut)value);
		}
	}

}
