shader_type spatial;
render_mode diffuse_toon, specular_toon;

uniform sampler2D block : filter_nearest, source_color;
uniform bool showBorder = false;

const float borderSize = .01;

void fragment() {
  bool isPixelInBorderX = UV.x < borderSize || (UV.x > (1. - borderSize));
  bool isPixelInBorderY = UV.y < borderSize || (UV.y > (1. - borderSize));
  bool isPixelInBorder = isPixelInBorderX || isPixelInBorderY;

  vec3 blockTexture = texture(block, UV).rgb;

  if (showBorder && isPixelInBorder) {
    ALBEDO = .01 + blockTexture * .1;
  } else {
    ALBEDO = blockTexture;
  }
}
