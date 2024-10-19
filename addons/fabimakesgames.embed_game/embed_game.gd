@tool
extends EditorPlugin

const debug: bool = false
var debugger: EditorDebugger
var sesh: EditorDebuggerSession
var last_main_screen_rect: Rect2i
var plugin_control: PanelContainer
var hbox: HBoxContainer
var activate_button: Button
var top_bar_button: Button
var last_main_screen_not_embed: String
var is_playing_scene: bool
var was_playing_scene: bool
var embed_game:= EmbedGame.new()
var cached_game_handle: int = 0


func _enter_tree():
	add_autoload_singleton("EmbedGameAutoload","res://addons/fabimakesgames.embed_game/embed_game_autoload.gd")
	
	## add checkbutton and reparent 'embed' button
	_add_control_elements()

	## CONNECT SIGNALS
	debugger = EditorDebugger.new()
	debugger.new_session.connect(register_debugger_session)
	debugger._on_return_focus.connect(self._on_return_focus)
	debugger._on_handle_transmitted.connect(self._on_handle_received)
	add_debugger_plugin(debugger)
	
	##connect main screen size to game window
	var main_screen := EditorInterface.get_editor_main_screen()
	main_screen_changed.connect(_on_main_screen_changed)
	
	#initialize vars
	was_playing_scene = EditorInterface.get_playing_scene() != ""
	is_playing_scene = EditorInterface.get_playing_scene() != ""
	
	if not ProjectSettings.has_setting("embed_game/padding"):
		ProjectSettings.set_setting("embed_game/padding", int(0))
		ProjectSettings.set_initial_value("embed_game/padding", int(0))
		ProjectSettings.save()
func _exit_tree():
	hbox.queue_free()
	remove_debugger_plugin(debugger)
	remove_autoload_singleton("EmbedGameAutoload")

	
func _build():
	cached_game_handle = 0 ## make sure the game handle will be gotten anew
	if activate_button.button_pressed:
		top_bar_button.visible = true
		EditorInterface.set_main_screen_editor("Embed")
	return true

func _on_activate_button_toggled(flag: bool):
	self.queue_save_layout() ## saves setting
	#if not is_playing_scene: return
	if flag:
		embed_window()
		if is_playing_scene:
			top_bar_button.visible = true
			EditorInterface.set_main_screen_editor("Embed")
			
	else:
		unembed_window()
		EditorInterface.set_main_screen_editor(last_main_screen_not_embed)
		plugin_control.visible = false
		top_bar_button.visible = false
		

## from project instance
func _on_handle_received(data: Array) -> void:
	cached_game_handle = data[0]
	if activate_button.button_pressed:
		embed_window()
		
func _get_window_layout(configuration: ConfigFile) -> void:
	configuration.set_value("embed_window", "is_enabled", activate_button.button_pressed)
	
func _set_window_layout(configuration: ConfigFile) -> void:
	activate_button.button_pressed = configuration.get_value("embed_window","is_enabled", false)

func _add_control_elements():
	var top_buttons:= get_top_buttons()
	for i in top_buttons:
		if i.text == "Embed":
			top_bar_button = i
	hbox = HBoxContainer.new()
	hbox.name = "Embed"
	last_main_screen_not_embed = top_buttons[0].name ## so it's never an empty string
	top_bar_button.get_parent().add_child(hbox)
	top_bar_button.reparent(hbox)
	top_bar_button.shortcut = load("res://addons/fabimakesgames.embed_game/config/embed_shortcut.tres")
	top_bar_button.visible = false
	activate_button = preload("res://addons/fabimakesgames.embed_game/embed_button.tscn").instantiate()
	activate_button.toggled.connect(_on_activate_button_toggled)
	hbox.add_child(activate_button,true)
	
	## add empty panel
	plugin_control = PanelContainer.new()
	EditorInterface.get_editor_main_screen().add_child(plugin_control)
	plugin_control.hide()
	

	
func _process(delta: float) -> void:
	if not activate_button.button_pressed: return
	
	## UPDATE PLACEMENT WHEN MOVING EDITOR WINDOW ONLY
	_update_screen_rect_if_required()
	
	## removes embed view on quitting the play mode
	is_playing_scene = EditorInterface.get_playing_scene() != ""
	
	if not is_playing_scene:
		cached_game_handle = 0 ## reset game handle e.g. no running instance.
		 
	if was_playing_scene and not is_playing_scene:
		top_bar_button.visible = false
		EditorInterface.set_main_screen_editor(last_main_screen_not_embed)
	was_playing_scene = is_playing_scene
	
func _on_return_focus(data):
	var keycode: int = data[0]
	var f_key_number = keycode - 4194332 
	var top_buttons:= get_top_buttons()
	if f_key_number < top_buttons.size():
		if f_key_number == 4: ## is KEY_F5
			EditorInterface.set_main_screen_editor("Embed")
		else:
			var desired_tab: String = top_buttons[f_key_number].name
			EditorInterface.set_main_screen_editor(desired_tab)

#region Window Management

func _update_screen_rect_if_required() -> void:
	var main_screen := EditorInterface.get_editor_main_screen()
	var main_screen_rect:= Rect2i(
		main_screen.global_position + Vector2.ONE * _get_padding() / 2,
		main_screen.size - Vector2.ONE * _get_padding()
	)
	if main_screen_rect != last_main_screen_rect:
		embed_game.set_window_rect(cached_game_handle, main_screen_rect)
		last_main_screen_rect = main_screen_rect
		
func _force_update_window_rect() -> void:
	var main_screen := EditorInterface.get_editor_main_screen()
	var main_screen_rect:= Rect2i(
		main_screen.global_position,
		main_screen.size
	)
	embed_game.set_window_rect(cached_game_handle, main_screen_rect)
	last_main_screen_rect = main_screen_rect
	
func get_handle_editor() -> int:
	var window := self.get_window().get_window_id()
	return DisplayServer.window_get_native_handle(DisplayServer.WINDOW_HANDLE, window)

func _get_padding() -> int:
	return ProjectSettings.get_setting("embed_game/padding", 2)

func embed_window() -> void:
	embed_game.show_window(cached_game_handle, false) ## takes longer but looks nicer
	embed_game.store_window_style(cached_game_handle)
	embed_game.set_window_borderless(cached_game_handle)
	embed_game.make_child(get_handle_editor(), cached_game_handle)
	_force_update_window_rect()
	embed_game.show_window(cached_game_handle, true)

	
func unembed_window() -> void:
	embed_game.show_window(cached_game_handle, false)
	embed_game.unmake_child(cached_game_handle) ##revert window style must come before unmake child, else window moves downwards
	embed_game.revert_window_style(cached_game_handle)
	embed_game.show_window(cached_game_handle, true)

func _on_main_screen_changed(screen_name: String) -> void:
	if screen_name != "Embed":
		last_main_screen_not_embed = screen_name
		if activate_button.button_pressed:
			embed_game.show_window(cached_game_handle,false)
	else:
		embed_game.show_window(cached_game_handle,true)


#region Helper Functions
func get_top_buttons() -> Array[Node]:
	var cont := Control.new()
	add_control_to_container(CustomControlContainer.CONTAINER_TOOLBAR, cont)
	var btns := cont.get_parent().get_child(2).get_children()
	remove_control_from_container(CustomControlContainer.CONTAINER_TOOLBAR, cont)
	return btns
#endregion


#region Editor Plugin specific functions
func _has_main_screen():
	return true
func _get_plugin_name():
	return "Embed"
func _make_visible(visible):
	plugin_control.visible = visible
func _get_plugin_icon():
	return preload("res://addons/fabimakesgames.embed_game/assets/icon.png")


#region Debugger Setup
class EditorDebugger extends EditorDebuggerPlugin:
	signal new_session(session:EditorDebuggerSession)
	signal _on_return_focus(session: EditorDebuggerSession)
	signal _on_handle_transmitted(session: EditorDebuggerSession)
	
	func _has_capture(prefix):
		if prefix == "return_focus": return true
		if prefix == "transmit_handle": return true
		return true
		
	func _capture(message, data, session_id):
		if message == "return_focus:":
			_on_return_focus.emit(data)
			return true
		if message == "transmit_handle:":
			_on_handle_transmitted.emit(data)
			return true
	func _setup_session(session_id):
		new_session.emit(get_session(session_id))
		
func register_debugger_session(dbgs: EditorDebuggerSession):
	sesh = dbgs
#endregion
