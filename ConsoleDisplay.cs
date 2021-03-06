using UnityEngine;
using UnityEngine.UI;
using System.Text;

namespace Hourai.Console {

	[RequireComponent(typeof(Text))]
	public class ConsoleDisplay : MonoBehaviour {

		private Text _displayText;
		private bool _updated;
		private StringBuilder _textBuilder;

		void Awake() {
			_textBuilder = new StringBuilder();
			_updated = false;
			GameConsole.OnConsoleUpdate += Redraw;
			_displayText = GetComponent<Text>();
		}

		void OnDestroy() {
			GameConsole.OnConsoleUpdate -= Redraw;
		}

		void OnEnable() {
			if(_updated)
				Redraw();
			_updated = false;
		}

		void Redraw() {
			if(!isActiveAndEnabled)  {
				_updated = true;
				return;
			}
            // Clears the current string
		    _textBuilder.Length = 0;
            foreach (string log in GameConsole.History) 
				_textBuilder.AppendLine(log);
			_displayText.text = _textBuilder.ToString();	
		}

	}

}
