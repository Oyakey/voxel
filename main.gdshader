shader_type spatial;
render_mode diffuse_toon, specular_toon;

uniform sampler2D block : filter_nearest, source_color;
uniform bool showBorder = false;

const float borderSize = .01;

// void vertex() {
// }

void fragment() {
  bool isPixelInBorder = (UV.x < borderSize) || (UV.x > (1. - borderSize)) || (UV.y < borderSize) || (UV.y > (1. - borderSize));

  if (showBorder && isPixelInBorder) {
    ALBEDO = vec3(0, 0, 0);
  } else {
    ALBEDO = texture(block, UV).rgb;
  }
}

//void light() {
//	// Called for every pixel for every light affecting the material.
//	// Uncomment to replace the default light processing function with this one.
//}
