shader_type fog;

uniform sampler2D noiseTex: repeat_enable;
uniform float noiseScale = 1.0;
uniform float flatness = 1.0;
uniform float density = 1.0;
uniform float speed = 1.0;
uniform sampler2D gradientTex;

void fog() {
	vec2 move_uv = vec2(0.2, 0.0) * TIME * speed;
	float detail_noise = texture(noiseTex, WORLD_POSITION.xz * noiseScale + move_uv * 0.5).r;
	detail_noise *= 0.2;
	float noise = texture(noiseTex, WORLD_POSITION.xz * noiseScale + move_uv + detail_noise).r;
	DENSITY = mix(1.0, noise, UVW.y * flatness);
	DENSITY *= step(0.0, -SDF) * density;
	vec3 color = texture(gradientTex, vec2(DENSITY, 0.5)).rgb;
	ALBEDO = color;
}
