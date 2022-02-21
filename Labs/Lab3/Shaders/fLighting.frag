#version 330

uniform vec4 uLightPosition;
uniform vec4 uEyePosition;

in vec4 oNormal;
in vec4 oSurfacePosition;

out vec4 FragColour;

void main()
{
	vec4 lightDir = normalize(uLightPosition - oSurfacePosition);
	vec4 eyeDirection = normalize(uEyePosition - oSurfacePosition);
	vec4 reflectedVector = reflect(-lightDir, oNormal);

	float specularFactor = pow(max(dot( reflectedVector, eyeDirection), 0.0), 30);
	float diffuseFactor = max(dot(oNormal, lightDir), 0);
	float ambientFactor = 0.1f;
	float totalFactor = specularFactor + diffuseFactor + ambientFactor;


	FragColour = vec4(vec3(totalFactor), 1);
}