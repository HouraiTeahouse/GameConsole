using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Hourai.Console {

	public delegate void ConsoleCommand(string[] args);

	public static class GameConsole {

		public static class Commands {

			public static void Clear(string[] args) {
				GameConsole.Clear();
			}

			public static void Echo(string[] args) {
				foreach(string arg in args)
					Log(arg);
			}

		}

		private static Dictionary<string, ConsoleCommand> _commands;
		private static FixedSizeQueue<string> _history;

		public static event Action OnConsoleUpdate;

		public static int HistorySize {
			get { return _history.Limit; }
			set { _history.Limit = value; }
		}

		public static IEnumerable History {
			get { return _history; }
		}

		static GameConsole() {
			_commands = new Dictionary<string, ConsoleCommand>();
		    _history = new FixedSizeQueue<string>(100);

            // Divert Debug Log messages to the GameConsole as well.
            Application.logMessageReceived += (log, stackTrace, type) => Log(log);

            RegisterCommand("echo", Commands.Echo);
            RegisterCommand("clear", Commands.Clear);
		}

	    static void UnityLog(string log, string stackTrace, LogType type) {
			Log(log);
		}

		public static void RegisterCommand(string command, ConsoleCommand callback) {
			if(!_commands.ContainsKey(command))
				_commands[command] = callback;
			else
				_commands[command] += callback;
		}

		public static bool UnregisterCommand(string command, ConsoleCommand callback) {
			if(!_commands.ContainsKey(command))
				return false;
			ConsoleCommand allCallbacks = _commands[command];
			allCallbacks -= callback;
		    if (allCallbacks == null)
		        _commands.Remove(command);
			else
				_commands[command] = allCallbacks;
			return true;
		}

		public static void Clear() {
			_history.Clear();
			if(OnConsoleUpdate != null)
				OnConsoleUpdate();
		}

		public static void Log(string message, params object[] objs) {
			if(message == null)
				message = string.Empty;
			_history.Enqueue(string.Format(message, objs));
			if(OnConsoleUpdate != null)
				OnConsoleUpdate();
		}

		public static void Execute(string command) {
			string[] args = command.Split(null);
			if(args.Length <= 0)
				return;
			if(!_commands.ContainsKey(args[0])) {
				Log("Command {0} does not exist.", args[0]);
				return;
			}
			ConsoleCommand cc = _commands[args[0]];
			Log(command);
			try {
				cc(args.Skip(1).ToArray());
			} catch { 
				Log("An error has occured.");
			}
		}
	}
}
