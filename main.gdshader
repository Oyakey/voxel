shader_type spatial;

uniform sampler2D block : filter_nearest, source_color;

void vertex() {
}

void fragment() {
  ALBEDO = texture(block, UV).rgb;
}

//void light() {
//	// Called for every pixel for every light affecting the material.
//	// Uncomment to replace the default light processing function with this one.
//}
