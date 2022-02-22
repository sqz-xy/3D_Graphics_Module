#version 330

uniform vec4 uLightPosition;
uniform vec4 uEyePosition;

in vec4 oNormal;
in vec4 oSurfacePosition;

out vec4 FragColour;

void main()
{
	vec4 lightDir = normalize(uLightPosition - oSurfacePosition);
	float diffuseFactor = max(dot(oNormal, lightDir), 0);
	FragColour = vec4(vec3(diffuseFactor), 1);
}