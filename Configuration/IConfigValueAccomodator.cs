using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Configuration
{
	/// <summary>
	/// Provide value accomodation for configuration keys
	/// </summary>
	public interface IConfigValueAccomodator
	{
		/// <summary>
		/// Provides value accomodation to configuration schema
		/// </summary>
		/// <param name="value">The value to accomodate</param>
		/// <returns>The accomodated value</returns>
		object Accomodate(object value);
		
		/// <summary>
		/// Returns the unaccomodated value
		/// </summary>
		/// <param name="value">The accomodated value</param>
		/// <returns>The unaccomodated value</returns>
		object AccomodateBack(object value);
	}

	/// <summary>
	/// Provides value accomodation to configuration schema, with strong type semantics
	/// </summary>
	/// <typeparam name="TIn">The unaccomodated value type</typeparam>
	/// <typeparam name="TOut">The accomodated value type</typeparam>
	public interface IConfigValueAccomodator<TIn, TOut> : IConfigValueAccomodator
	{
		/// <summary>
		/// Provides value accomodation to configuration schema
		/// </summary>
		/// <param name="value">The value to accomodate</param>
		/// <returns>The accomodated value</returns>
		TOut Accomodate(TIn value);

		/// <summary>
		/// Returns the unaccomodated value
		/// </summary>
		/// <param name="value">The accomodated value</param>
		/// <returns>The unaccomodated value</returns>
		TIn AccomodateBack(TOut value);
	}

	/// <summary>
	/// Provides basic implementation if IConfigValueAccomodator&lt;TIn, TOut%gt;
	/// </summary>
	/// <typeparam name="TIn">The unaccomodated value type</typeparam>
	/// <typeparam name="TOut">The accomodated value type</typeparam>
	public abstract class ConfigValueAccomodator<TIn, TOut> : IConfigValueAccomodator<TIn, TOut>
	{
		/// <summary>
		/// Provides value accomodation to configuration schema
		/// </summary>
		/// <param name="value">The value to accomodate</param>
		/// <returns>The accomodated value</returns>
		public abstract TOut Accomodate(TIn value);

		/// <summary>
		/// Returns the unaccomodated value
		/// </summary>
		/// <param name="value">The accomodated value</param>
		/// <returns>The unaccomodated value</returns>
		public abstract TIn AccomodateBack(TOut value);

		/// <summary>
		/// Provides value accomodation to configuration schema
		/// </summary>
		/// <param name="value">The value to accomodate</param>
		/// <returns>The accomodated value</returns>
		public object Accomodate(object value)
		{
			return Accomodate((TIn)value);
		}

		/// <summary>
		/// Returns the unaccomodated value
		/// </summary>
		/// <param name="value">The accomodated value</param>
		/// <returns>The unaccomodated value</returns>
		public object AccomodateBack(object value)
		{
			return AccomodateBack((TOut)value);
		}
	}

}
