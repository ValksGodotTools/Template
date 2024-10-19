extends Node

func _ready() -> void:
	if !OS.has_feature("editor"): ## remove plugin autoloads in exported builds
		queue_free()

	EngineDebugger.send_message("transmit_handle:",[DisplayServer.window_get_native_handle(DisplayServer.WINDOW_HANDLE, self.get_window().get_window_id())])

func _input(event: InputEvent) -> void:
	if event is InputEventKey:
		if event.ctrl_pressed and event.pressed:
				if event.keycode >= KEY_F1 and event.keycode <= KEY_F9:
					EngineDebugger.send_message("return_focus:", [event.keycode])
					
