using UnityEngine;
using System.Collections;

public class MInput
{
	public static int KEYBOARD1 = 1;
	public static int KEYBOARD2 = 2;
	public static int MIXED = 3;


	public static int CONTROL = KEYBOARD1;

	public static bool fwd {
		get {
			if (MInput.CONTROL <= KEYBOARD2) {
				return Input.GetKey (KeyCode.W);

//				Debug.Log(Input.GetAxis ("Vertical"));
//				return Input.GetAxis ("Vertical") > 0;
			}
			else
				return Input.GetAxis ("Vertical") > 0;
		}
	}

	public static bool bck {
		get {
			if (MInput.CONTROL <= KEYBOARD2)
				return Input.GetKey (KeyCode.S);
			else
				return false;
		}
	}

	public static bool sleft {
		get {
			if (MInput.CONTROL <= KEYBOARD2)
				return Input.GetKey (KeyCode.A);
			else
				return false;
		}
	}

	public static bool sright {
		get {
			if (MInput.CONTROL <= KEYBOARD2)
				return Input.GetKey (KeyCode.D);
			else
				return false;
		}
	}

	public static bool rleft {
		get {
			if (MInput.CONTROL <= KEYBOARD2)
				return Input.GetKey (KeyCode.Q);
			else
				return false;
		}
	}

	public static bool rright {
		get {
			if (MInput.CONTROL <= KEYBOARD2)
				return Input.GetKey (KeyCode.E);
			else
				return false;
	
		}
	}

	public static bool space {
		get {
			if (MInput.CONTROL <= KEYBOARD2)
				return Input.GetKey (KeyCode.W);
			else
				return false;
		}
	}


}
