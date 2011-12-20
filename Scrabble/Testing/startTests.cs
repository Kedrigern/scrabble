using System;
using System.IO;

namespace Scrabble.Testing
{
	/// <summary>
	/// Tests.
	/// </summary>
	public static class Tests
	{			
		/// <summary>
		/// Start tests.
		/// </summary>
		public static void start ()
		{					
			var d = new Dictionary();
			d.MakeTests();
		}
	}
	
	
	/// <summary>
	/// Dictionary tests
	/// </summary>
	/// <exception cref='Exception'>
	/// Represents errors that occur during application execution.
	/// </exception>
	public class Dictionary {
		
		Scrabble.Lexicon.GADDAG dic1;
		Scrabble.Lexicon.GADDAG dic2;
		
		/// <summary>
		/// Makes the tests.
		/// </summary>
		/// <returns>
		/// The tests.
		/// </returns>
		/// <exception cref='Exception'>
		/// Represents errors that occur during application execution.
		/// </exception>
		public bool MakeTests() {
			
			/* CONSTRUCTORS */
			Console.Out.Write("Vytvářím slovník (bezparametrický konstruktor):\t");
			dic1 = new Scrabble.Lexicon.GADDAG();
			Console.Out.WriteLine("OK");
			
			string s1 = "dům";
			string s2 = "dort";		
			Console.Out.Write("Vytvářím slovník (\"dům\",\"dort\"):\t");
			dic2 = new Scrabble.Lexicon.GADDAG(new string [] {s1, s2} );
			if( dic2.Content(s1) ) {
				//ok
			} else throw new Exception("FAIL GADDAG constructor with parametr string[]");	
			Console.Out.WriteLine("OK");
			
			/* ADD */
			Console.Out.Write("Přidávám do slovníku:\t");	
			string s3 = "důl";
			string s4 = "kůl";
			dic1.Add( s3 );
			dic1.Add( s4 );
			
			if( dic1.Content(s3) ) {
				//ok
			} else throw new Exception("FAIL add to dictionary");	
			Console.Out.WriteLine("OK");
			
			
			/* REMOVE */
			
			
			return true;
		}
	}
}

