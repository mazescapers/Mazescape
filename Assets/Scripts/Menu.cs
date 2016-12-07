using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// This class should generally be a component on a text object, within a canvas.
// Submenus should be other menu-text objects placed as children of this menu.
public class Menu : MonoBehaviour
{
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
	private UnityEngine.UI.Text mText = null;
	// This menu's text object, if it has one.
	private List<Transform> submenuList;
	// Recursive submenu children.
	private Menu menuParent;
	// This menu's parent menu, if it has one.
	private Color highlightColor, normalColor;
	// Colors for text.

	// Use this for initialization
	void Awake ()
	{
		// Set the parent, but only if it is also a menu
		if (transform.parent != null) {
			menuParent = transform.parent.GetComponent<Menu> ();
		} else {
			menuParent = null;
		}
		// This menu is a root menu if it has no parent or its parent isn't a menu.
		isRoot = menuParent == null;
		submenuList = new List<Transform> ();
		for (int i = 0; i < transform.childCount; i++) {
			// Set up submenus from children
			Transform thisChild = transform.GetChild (i);
			Menu thisMenu = thisChild.GetComponent<Menu> ();
			// If the child has no Menu component, ignore it
			if (thisMenu != null) {
				//Debug.Log ("Adding " + name + " submenu: " + thisChild.name);
				submenuList.Add (thisChild);
			}
		}
		// It is a leaf if it has no menu children.
		isLeaf = submenuList.Count == 0;
		if (mText == null) {
			mText = GetComponent<UnityEngine.UI.Text> ();
		}
		// Only display a root menu if it must be selected before displaying options;
		// So if a root menu is not clickable, it will by default just display its own children.
		// For example, "PAUSE" would be a root and clickable, and would also display a submenu,
		// An example of the opposite might be a start menu, which displays multiple options right
		// after loading (like "Join" and "Host").
		// Currently, isDisplayed isn't really used much at all, but it might be handy to keep
		// track of at some point. If the object has text, isDisplayed should equal text.enabled.
		isDisplayed = isRoot && (isClickable || isLeaf);
	}

	void Start ()
	{
		if (mText != null) {
			// If it has text, save its color...
			normalColor = mText.color;
			// ...hardcode the selection color for now...
			highlightColor = Color.green;
			// ...and enable/disable the text accordingly
			//Debug.Log(name + "'s mText is not null and it is displayed: " + isDisplayed);
			SetDisplayed (isDisplayed);
		}
		if (isRoot && (isClickable || isLeaf)) {
			//Debug.Log (this.name + " is displayed");
			// If it is a clickable root, hide all submenus
			CloseMenuChildrenRecursively ();
			ShowMenu ();
		} else if (isRoot) {
			//Debug.Log (this.name + " is not displayed and is a root");
			OpenMenuChildren ();
		}
	}

	void Update ()
	{
		// Doesn't need to update anything
	}

	// Every non-leaf menu in a menu tree that is clickable should call this function
	// on the clicked event (in its EventTrigger component).
	// Leaves probably should too, if you want to hide them after they perform
	// their action.
	// Pause menu example:
	// PAUSED (root, clickable, triggers pause)
	// (child) UNPAUSE (leaf, clickable, isBackButton, triggers unpause)
	public void Clicked ()
	{
		if (isDisplayed && isClickable) {
			// Being a back button makes it back up even if there are submenus.
			// So any submenus will be completely ignored.
			if (isRecursiveBackButton) {
				BackUpRecursively ();
			} else if (isBackButton) {
				BackUp ();
			} else if (isLeaf) {
				// Does its action; this should be set as an event in the editor.
				HideMenu (); // Might be times we don't want to hide it after the action?
			} else {
				// Open its submenu; i.e. display the children and hide this menu option's text.
				if (menuParent != null) {
					menuParent.CloseMenuChildren ();
				}
				OpenMenuChildren ();
			}
		}
	}

	// Closes all child menus.
	public void CloseMenuChildren ()
	{
		foreach (Transform menuObj in submenuList) {
			// Get the child's menu
			Menu thisMenu = menuObj.GetComponent<Menu> ();
			// If the child has text, hide it
			thisMenu.HideMenu ();
		}
	}

	// Hides all children, and their children, etc.
	// Does not show/hide this menu.
	void CloseMenuChildrenRecursively ()
	{
		foreach (Transform menuObj in submenuList) {
			// Get the child's menu
			Menu thisMenu = menuObj.GetComponent<Menu> ();
			// If the child has text, hide it
			thisMenu.HideMenu ();
			// Then close the children's children
			thisMenu.CloseMenuChildrenRecursively ();
		}
	}

	// Hides this menu's children and backs up to the parent menu, if it exists
	void BackUp ()
	{
		if (menuParent != null) {
			menuParent.CloseMenuChildren ();
			if (menuParent.GetParent () != null) {
				menuParent.GetParent ().OpenMenuChildren ();
			} else {
				menuParent.ShowMenu ();
			}
		}
	}

	public Menu GetParent ()
	{
		return menuParent;
	}

	// Moves up to the highest ancestor (the menu root) then shows that root
	// and closes all submenus.  i.e. it goes back to the start of the menu.
	void BackUpRecursively ()
	{
		if (menuParent != null) {
			menuParent.BackUpRecursively ();
		} else {
			if (isClickable) {
				ShowMenu ();
				CloseMenuChildrenRecursively ();
			} else {
				CloseMenuChildrenRecursively ();
				OpenMenuChildren ();
			}
		} 
	}

	// Shows all children, and hides this menu
	void OpenMenuChildren ()
	{
		foreach (Transform menuObj in submenuList) {
			//Debug.Log ("Checking " + menuObj.name);
			Menu thisMenu = menuObj.GetComponent<Menu> ();
			UnityEngine.UI.Text thisText = thisMenu.mText;
			// If the child has text, display it
			if (thisText == null) {
				thisMenu.mText = thisMenu.GetComponent<UnityEngine.UI.Text> ();
			}
			if (thisText != null) {
				thisMenu.isDisplayed = true;
				thisText.enabled = true;
			}
		}
		HideMenu ();
	}

	// Sets the 'isDisplayed' variable, and if this menu has text, it shows/hides it too
	void SetDisplayed (bool disp)
	{
		//Debug.Log ("Setting visibility of " + this.name + " to " + disp);
		isDisplayed = disp;
		if (mText != null) {
			mText.enabled = disp;
		}
	}

	void HighlightText ()
	{
		if (mText != null) {
			mText.color = highlightColor;
		}
	}

	void UnhighlightText ()
	{
		if (mText != null) {
			mText.color = normalColor;
		}
	}

	void HideMenu ()
	{
		SetDisplayed (false);
	}

	void ShowMenu ()
	{
		SetDisplayed (true);
	}

	public void Hover ()
	{
		if (isClickable && mText != null) {
			mText.color = highlightColor;
		}
	}

	public void Unhover ()
	{
		if (isClickable && mText != null) {
			mText.color = normalColor;
		}
	}
}
