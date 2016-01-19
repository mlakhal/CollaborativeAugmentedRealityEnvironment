using UnityEngine;
using System.Collections;

// Common view interface:
public interface View {
	
	// Rendering UI function:
	void render();
	
	// Enable/disable UI components:
	void setBlockUI(bool blockUI);
	
}
