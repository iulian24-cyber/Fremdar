shader_type spatial;
render_mode cull_disabled, unshaded;
uniform float scale = 300;
uniform float speed = 1;
uniform float wiggle = 100;

uniform float dropStrength = 2;
uniform float trailStrength = 2;
uniform float blur = 3;
uniform float fadeDistance = 5;
uniform float layers = 3;

uniform sampler2D screen_tex : hint_screen_texture, filter_linear_mipmap;

float RandomNumber(vec2 seed){
	seed = fract(seed * vec2(123.456, 234.789));
	seed += dot(seed, seed + 34.56);
	return fract(seed.x * seed.y);
}

vec2 raindrops(vec2 uv, float dropScale, float dropSpeed, float dropWiggle){
	float t = TIME * dropSpeed;
	float by = (1.0 - uv.y) * 0.3 + 0.7;
	float w = uv.y * dropWiggle;
	uv *= dropScale;
	uv.y -= t * 0.25;
	vec2 grid_uv = fract(uv) - 0.5;
	vec2 id = floor(uv);

	float rn = RandomNumber(id);
	t += rn * TAU;
	w *= rn;

	float dr = 0.1 * by;
	float doffs = 0.5 - dr;
	float x = (rn - 0.5) * doffs;
	x += sin(w + cos(w + 4.0) * pow(sin(x), 2.0)) * (doffs - abs(x));
	float y = cos(t + cos(t + cos(t) * 0.5)) * doffs;
	y += (grid_uv.x - x) * (grid_uv.x - x);
	vec2 dpos = grid_uv - vec2(x, y * speed);
	float drop = smoothstep(dr, 0.01, length(dpos));

	float trail = smoothstep(dr, -dr, dpos.y);
	trail *= smoothstep(dr, 0.01, abs(dpos.x));
	trail *= smoothstep(-0.5, y, grid_uv.y);

	float fade = 1.0 - clamp(fwidth(uv.x) * fadeDistance, 0, 1);

	vec2 output = vec2(0);
	output += trail * trailStrength * by;
	output += drop * dropStrength * by;

	return output * dpos * 10.0 * fade;
}

void fragment() {
	vec2 droplets = vec2(0);
	for(float i = 1.0; i <= float(layers); i++)
		droplets += raindrops(UV, scale * i * 0.2, speed * i * 1.5, wiggle * i * 0.5);
	ALBEDO = textureLod(screen_tex, SCREEN_UV - droplets, smoothstep(0.01, 0.001, droplets.x) * blur).rgb;
	ALPHA = droplets.r;
}