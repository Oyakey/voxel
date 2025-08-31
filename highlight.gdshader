shader_type spatial;
render_mode diffuse_toon, specular_toon;

uniform bool showBorder = false;

void fragment() {
  float distanceFromCamera = length(VIEW);
  float borderSize = .2 * distanceFromCamera;

  bool isPixelInBorder = (UV.x < borderSize) || (UV.x > (1. - borderSize)) || (UV.y < borderSize) || (UV.y > (1. - borderSize));
  if (showBorder && isPixelInBorder) {
    ALBEDO = vec3(0, 0, 0);
  }
}

//void light() {
//	// Called for every pixel for every light affecting the material.
//	// Uncomment to replace the default light processing function with this one.
//}
