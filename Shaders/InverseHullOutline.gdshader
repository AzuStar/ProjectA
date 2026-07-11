// Inverted hull outline pass for Godot 4.x.
// Assign as a material next_pass so the base surface still renders normally.
// Based on CC0 GodotShaders inverted-hull/group-merge outline technique.
shader_type spatial;
render_mode cull_front, unshaded, blend_mix, depth_draw_opaque, shadows_disabled;

uniform vec4 outline_color : source_color = vec4(0.02, 0.02, 0.025, 1.0);
uniform float thickness : hint_range(0.0, 0.30, 0.001) = 0.035;
uniform float outline_energy : hint_range(0.0, 4.0, 0.01) = 1.0;
uniform float outline_transparency : hint_range(0.0, 1.0, 0.01) = 1.0;

uniform bool use_vertex_normals = true;
uniform vec3 radial_origin = vec3(0.0, 0.05, 0.0);

uniform bool merge_group = false;
uniform float merge_depth_range : hint_range(0.1, 100.0, 0.1) = 10.0;
uniform sampler2D depth_texture : hint_depth_texture, repeat_disable, filter_linear_mipmap;

varying float expanded_view_z;

void vertex() {
	vec3 object_dir = NORMAL;
	if (!use_vertex_normals) {
		vec3 radial = VERTEX - radial_origin;
		object_dir = length(radial) > 0.001 ? normalize(radial) : NORMAL;
	}

	vec3 view_dir = normalize((MODELVIEW_MATRIX * vec4(object_dir, 0.0)).xyz);
	vec4 view_position = MODELVIEW_MATRIX * vec4(VERTEX, 1.0);
	view_position.xyz += view_dir * thickness;
	expanded_view_z = view_position.z;
	POSITION = PROJECTION_MATRIX * view_position;
}

void fragment() {
	if (merge_group) {
		float raw_depth = texture(depth_texture, SCREEN_UV).r;
		vec4 view_space = INV_PROJECTION_MATRIX * vec4(SCREEN_UV * 2.0 - 1.0, raw_depth, 1.0);
		float scene_distance = -(view_space.xyz / view_space.w).z;
		float fragment_distance = -expanded_view_z;
		float depth_gap = scene_distance - fragment_distance;

		if (depth_gap > 0.01 && depth_gap < merge_depth_range) {
			discard;
		}
	}

	ALBEDO = outline_color.rgb * outline_energy;
	ALPHA = outline_color.a * outline_transparency;
}
