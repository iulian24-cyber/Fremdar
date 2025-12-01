shader_type spatial;
render_mode cull_disabled;

uniform float scale = 1.0;
uniform float fadeDistance = 0.5;


vec2 Grid(vec2 uv){
	uv *= scale;
	uv.y -= TIME;
	uv.x -= TIME / 2.0;
	vec2 grid_uv = fract(uv) - 0.5;
	vec2 output = vec2(0);

	float fade = 1.0 - clamp(fwidth(uv.x) * fadeDistance, 0, 1);

	if(grid_uv.x > 0.45 || grid_uv.y > 0.45) output.r = 1.0;
	return output  * fade;
}

void fragment() {
	vec3 color = Grid(UV).rrr;
	ALBEDO = color.rrr;
	ALPHA = color.r;
}