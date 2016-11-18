using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// This class should generally be a component on a text object, within a canvas.
// Submenus should be other menu-text objects placed as children of this menu.
public class Menu : MonoBehaviour {
	// Things like the pause menu need to be given separate locations for the different players.
	// Set to null to make them always be in the same place (where they start).
	public GameObject locationForStrategist, locationForGround;
	// Clickable means it can be selected when visible, like most menu options.
	// Clickable would be false if you wanted, say, a paragraph of instructional
	// text above one menu.
	// Back buttons back up one step in the menu system when clicked.
	// Recursive back buttons (higher priority than normal ones) back up to the start.
	public bool isClickable, isBackButton, isRecursiveBackButton;

	private bool isLeaf, isRoot, isDisplayed;
	private UnityEngine.UI.Text mText; // This menu's text object, if it has one.
	private List<GameObject> submenuList; // Recursive submenu children.
	private Menu menuParent; // This menu's parent menu, if it has one.
	private Color highlightColor, normalColor; // Colors for text.

	// Use this for initialization
	void Start () {
		// Set the parent, but only if it is also a menu
		if (transform.parent != null) {
			menuParent = transform.parent.GetComponent<Menu> ();
		} else {
			menuParent = null;
		}
		// This menu is a root menu if it has no parent or its parent isn't a menu.
		isRoot = menuParent == null;
		for (int i = 0; i < transform.childCount; i++) {
			// Set up submenus from children
			GameObject thisChild = transform.GetChild(i);
			Menu thisMenu = thisChild.GetComponent<Menu>();
			// If the child has no Menu component, ignore it
			if (thisMenu != null) {
				submenuList.Add(thisChild);
			}
		}
		// It is a leaf if it has no menu children.
		isLeaf = submenuList.Count == 0;
		UnityEngine.UI.Text mText = GetComponent<UnityEngine.UI.Text> ();
		// Only display a root menu if it must be selected before displaying options;
		// So if a root menu is not clickable, it will by default just display its own children.
		// For example, "PAUSE" would be a root and clickable, and would also display a submenu,
		// An example of the opposite might be a start menu, which displays multiple options right
		// after loading (like "Join" and "Host").
		// Currently, isDisplayed isn't really used much at all, but it might be handy to keep
		// track of at some point. If the object has text, isDisplayed should equal text.enabled.
		isDisplayed = isRoot && (isClickable || isLeaf);
		if (mText != null) {
			// If it has text, save its color...
			normalColor = mText.color;
			// ...hardcode the selection color for now...
			highlightColor = Color.green;
			// ...and enable/disable the text accordingly
			mText.enabled = isDisplayed;
		}
		if (isDisplayed) {
			// If it is a clickable root, hide all submenus
			CloseMenuChildrenRecursively ();
			ShowMenu ();
		}
	}

	void Update () {
		// Doesn't need to update anything
	}

	// Every non-leaf menu in a menu tree that is clickable should call this function
	// on the clicked event (in its EventTrigger component).
	// Leaves probably should too, if you want to hide them after they perform
	// their action.
	// Pause menu example:
	// PAUSED (root, clickable, triggers pause)
	// (child) UNPAUSE (leaf, clickable, isBackButton, triggers unpause)
	public void Clicked () {
		if (isDisplayed && isClickable) {
			// Being a back button makes it back up even if there are submenus.
			// So any submenus will be completely ignored.
			if (isRecursiveBackButton) {
				BackUpRecursively ();
			} else if (isBackButton) {
				BackUp ();
			} else if (isLeaf) {
				// Does its action; this should be set as an event in the editor.
				HideMenu(); // Might be times we don't want to hide it after the action?
			} else {
				// Open its submenu; i.e. display the children and hide this menu option's text.
				OpenMenuChildren ();
			}
		}
	}

	// Closes all child menus.
	void CloseMenuChildren() {
		foreach (GameObject menuObj in submenuList) {
			// Get the child's menu
			Menu thisMenu = menuObj.GetComponent<Menu>();
			// If the child has text, hide it
			thisMenu.HideMenu();
		}
	}

	// Hides all children, and their children, etc.
	// Does not show/hide this menu.
	void CloseMenuChildrenRecursively() {
		foreach (GameObject menuObj in submenuList) {
			// Get the child's menu
			Menu thisMenu = menuObj.GetComponent<Menu>();
			// If the child has text, hide it
			thisMenu.HideMenu();
			// Then close the children's children
			thisMenu.CloseMenuChildrenRecursively();
		}
	}

	// Hides this menu's children and backs up to the parent menu, if it exists
	void BackUp() {
		if (menuParent != null) {
			menuParent.ShowMenu ();
			menuParent.CloseMenuChildrenRecursively ();
		}
	}

	// Moves up to the highest ancestor (the menu root) then shows that root
	// and closes all submenus.  i.e. it goes back to the start of the menu.
	void BackUpRecursively() {
		if (menuParent != null) {
			menuParent.BackUpRecursively ();
		} else {
			ShowMenu();
			CloseMenuChildrenRecursively ();
		} 
	}

	// Shows all children, and hides this menu
	void OpenMenuChildren() {
		foreach (GameObject menuObj in submenuList) {
			Menu thisMenu = menuObj.GetComponent<Menu>();
			UnityEngine.UI.Text thisText = thisMenu.mText;
			// If the child has text, display it
			if (mText != null) {
				thisMenu.isDisplayed = true;
				thisText.enabled = true;
			}
		}
		HideMenu ();
	}

	// Sets the 'isDisplayed' variable, and if this menu has text, it shows/hides it too
	void SetDisplayed(bool disp) {
		isDisplayed = disp;
		if (mText != null) {
			mText.enabled = disp;
		}
	}

	void HighlightText() {
		if (mText != null) {
			mText.color = highlightColor;
		}
	}

	void UnhighlightText() {
		mif (mText != null) {
			mText.color = normalColor;
		}
	}
		
	void HideMenu() {
		SetDisplayed (false);
	}

	void ShowMenu() {
		SetDisplayed (true);
	}
}
