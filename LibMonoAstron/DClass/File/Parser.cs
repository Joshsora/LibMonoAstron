using System;
using System.Text.RegularExpressions;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace Astron
{
	namespace DClass
	{
		public class Parser
		{
			// exceptions
			class DCException : Exception
			{
				public DCException(string message)
					: base(message)
				{
				}
			}

			public static class Tokens
			{
				// Parsing
				public const string PYTHON_FROM = "from";
				public const string PYTHON_IMPORT = "import";
				public const string STRUCT = "struct";
				public const string DCLASS = "dclass";
				public const string TYPEDEF = "typedef";
				public const string KEYWORD = "keyword";
				
				// Default Keywords
				public static readonly string[] keywords = new string[] {
					"broadcast",
					"required",
					"ram",
					"db",
					"clrecv",
					"ownrecv",
					"airecv",
					"clsend",
					"ownsend"
				};

				// Default Types
				public static readonly string[] types = new string[] {
					"int8",
					"int16",
					"int32",
					"int64",
					
					"uint8",
					"uint16",
					"uint32",
					"uint64",
					
					"char",
					"float32",
					"float64",
					
					"string",
					"blob"
				};
			}

			enum ParserState
			{
				COMMENT,
				OUTSIDE,
				INSIDE
			}

			string line = "";
			List<string> lines;

			ParserState state;
			ParserState previousState;

			int cIndex = 0;
			int lIndex = 0;

			File file;
			Struct currentStruct;
			Class currentClass;

			// Syntax checking
			bool IsValidIdentifier(string name)
			{
				// Names can't be more than one word
				if (name.Contains(" "))
				{
					return false;
				}

				// Names can't start with a number
				if (Regex.IsMatch(name, @"^\d+"))
				{
					return false;
				}

				return true;
			}

			// Error reporting
			void ThrowError(string message)
			{
				message = string.Format("Error on line {0} while parsing, {1}", lIndex, message);
				throw new DCException(message);
			}

			// Type handlers

			// Property handlers
			void HandleDefaultValue(string origName, out string name, out string defaultValue)
			{
				name = origName;
				defaultValue = origName;
			}

			// Junk handlers
			void HandleTheRest()
			{
				string theRest = ReadToEnd();
				if (theRest != "")
				{
					if (!theRest.StartsWith("//"))
					{
						if (theRest.StartsWith("/*"))
						{
							SetState(ParserState.COMMENT, ParserState.INSIDE);
							if (line.Contains("*/"))
							{
								HandleComment();
							}
							return;
						}
						ThrowError("unexpected mess after '{'.");
					}
				}
			}

			// Field handlers
			void HandleField(string type)
			{
				// We need a name for this field.
				string name = ReadUpTo(';', true);
				string defaultVal = "";

				// Check for default value
				if (name.Contains(" "))
				{
					HandleDefaultValue(name, out name, out defaultVal);
				}
			}

			void HandleAtomicField(string name)
			{
			}

			void HandleMolecularField(string name)
			{
			}

			// Token handlers
			void HandleTypedef()
			{

			}

			void HandleKeyword()
			{

			}

			void HandleDClass()
			{

			}

			void HandleStruct()
			{
				// Struct name
				string[] s_structName = ReadUpToEither(new char[] { ' ', '{' }, true);
				string structName = s_structName [0];

				// syntax checks
				if (s_structName [1] == " ")
				{
					string mess = ReadUpTo('{', true);
					if (mess != "")
					{
						structName += " " + mess;
					}
				}

				if (!IsValidIdentifier(structName))
				{
					ThrowError("Struct name '" + structName + "' was invalid.");
				}

				HandleTheRest();

				// Add the struct to the file
				currentStruct = new Struct(file, structName);
				file.AddStruct(currentStruct);

				// Set Parser state
				SetState(ParserState.INSIDE);
			}

			// State handlers
			void HandleComment()
			{
				try
				{
					ReadUpTo("*/", true);
					ReturnToPreviousState();
				} catch (DCException e)
				{
					// Wasn't on this line..
					return;
				}
			}

			void HandleOutside()
			{
				string token = ReadUpTo(' ', true);
				switch (token)
				{
					case (Tokens.PYTHON_FROM):
						// We don't care but it's valid..
						break;
					
					case (Tokens.PYTHON_IMPORT):
						// We don't care but it's valid..
						break;

					case (Tokens.TYPEDEF):
						HandleTypedef();
						break;

					case (Tokens.KEYWORD):
						HandleKeyword();
						break;

					case (Tokens.DCLASS):
						HandleDClass();
						break;

					case (Tokens.STRUCT):
						HandleStruct();
						break;
					
					default:
						ThrowError("encountered unknown token in OUTSIDE state.");
						break;
				}
			}

			void HandleInside()
			{
				string[] s_first = ReadUpToEither(new char[] { ' ', '(', ':' }, true);
				if (s_first [1] == " ")
				{
					if (line [cIndex] == ':')
					{
						cIndex++;
						if (currentClass != (Class)null)
						{
							HandleMolecularField(s_first [0]);
						} else
						{
							ThrowError("Structs cannot have molecular fields.");
						}
					} else
					{
						HandleField(s_first [0]);
					}
				} else if (s_first [1] == "(")
				{
					if (currentClass != (Class)null)
					{
						HandleAtomicField(s_first [0]);
					} else
					{
						ThrowError("Structs cannot have atomic fields.");
					}
				} else
				{
					if (currentClass != (Class)null)
					{
						HandleMolecularField(s_first [0]);
					} else
					{
						ThrowError("Structs cannot have molecular fields.");
					}
				}
			}

			// Cleaning
			string CleanLine(string line)
			{
				// Remove tabs and initial spaces
				int spaces = 0;
				
				line = line.Replace("\t", "");
				if (line != string.Empty)
				{
					for (int i = 0; i < line.Length; i++)
					{
						if (line [i] != ' ')
						{
							break;
						}
						
						spaces += 1;
					}
					
					if (spaces > 0)
					{
						line = line.Remove(0, spaces);
					}
				}
				
				return line;
			}

			// Lexing
			string ReadUpTo(char del = ' ', bool clean = false)
			{
				string temp = "";
				int spaces = 0;
				
				if (clean)
				{
					for (int i = cIndex; i < line.Length; i++)
					{
						if (line [i] != ' ')
						{
							break;
						}
						
						spaces += 1;
					}
					
					if (spaces > 0)
					{
						line = line.Remove(cIndex, spaces);
					}
				}
				
				if (line.Substring(cIndex).Contains(del))
				{
					while (line[cIndex] != del)
					{
						temp += line [cIndex];
						cIndex++;
					}
				} else
				{
					ThrowError("syntax error.");
				}
				
				cIndex++;
				cIndex += spaces;
				return temp;
			}

			string ReadUpTo(string del, bool clean = true)
			{
				string temp = "";
				int spaces = 0;
				
				if (clean)
				{
					for (int i = cIndex; i < line.Length; i++)
					{
						if (line [i] != ' ')
						{
							break;
						}
						
						spaces += 1;
					}
					
					if (spaces > 0)
					{
						line = line.Remove(cIndex, spaces);
					}
				}
				
				if (line.Substring(cIndex).Contains(del))
				{
					while (line.Substring(cIndex, del.Length) != del)
					{
						temp += line [cIndex];
						cIndex++;
					}
				} else
				{
					ThrowError("syntax error.");
				}
				
				cIndex++;
				cIndex += spaces;
				return temp;
			}
			
			string[] ReadUpToEither(char[] dels, bool clean = false)
			{
				string temp = "";
				bool contains = false;
				int spaces = 0;
				
				if (clean)
				{
					for (int i = cIndex; i < line.Length; i++)
					{
						if (line [i] != ' ')
						{
							break;
						}
						
						spaces += 1;
					}
					
					if (spaces > 0)
					{
						line = line.Remove(cIndex, spaces);
					}
				}
				
				for (int i = 0; i < dels.Length; i++)
				{
					if (line.Substring(cIndex).Contains(dels [i]))
					{
						contains = true;
						break;
					}
				}
				
				if (!contains)
				{
					ThrowError("syntax error.");
				}
				
				while (!dels.Contains(line[cIndex]))
				{
					temp += line [cIndex];
					cIndex++;
				}
				
				cIndex++;
				cIndex += spaces;
				return new string[] { temp, line [cIndex - 1 - spaces].ToString() };
			}

			string ReadToEnd()
			{
				string toEnd = CleanLine(line.Substring(cIndex));
				cIndex = line.Length;
				return toEnd;
			}

			// State
			void SetState(ParserState _state)
			{
				previousState = state;
				state = _state;
			}

			void SetState(ParserState _state, ParserState nextState)
			{
				previousState = nextState;
				state = _state;
			}

			void ReturnToPreviousState()
			{
				ParserState prv = state;
				state = previousState;
				previousState = prv;
			}

			// Parsing
			void ParseLine()
			{
				// Reset cIndex
				cIndex = 0;

				// Check if the line starts with a comment..
				if (line.StartsWith("/*"))
				{
					SetState(ParserState.COMMENT);
				}

				// Empty?
				if (line == "")
				{
					return;
				}

				// Switch depending on current state
				switch (state)
				{
					case (ParserState.COMMENT):
						HandleComment();
						break;
						
					case (ParserState.OUTSIDE):
						HandleOutside();
						break;
						
					case (ParserState.INSIDE):
						HandleInside();
						break;
				}
			}

			public void Parse(string[] files, File _file)
			{
				// Reference to the file
				file = _file;

				// Read the file(s)
				for (int i = 0; i < files.Length; i++)
				{
					state = ParserState.OUTSIDE;
					lines = new List<string>();

					using (StreamReader sr = new StreamReader (files[i]))
					{
						// Store the line we're cleaning
						string l;

						// Look through the lines in the file
						while ((l = sr.ReadLine()) != null)
						{
							// Remove initial whitespace
							l = CleanLine(l);

							// Remove single-line comments
							if (l.StartsWith("//"))
							{
								lines.Add("");
								continue;
							}

							// Add the line
							lines.Add(l);
						}

						// Close the file
						sr.Close();
					}

					// Loop through the lines and parse the data
					for (lIndex = 0; lIndex < lines.Count; lIndex++)
					{
						line = lines [lIndex];
						ParseLine();
					}
				}
			}
		}
	}
}
