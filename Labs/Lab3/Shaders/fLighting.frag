#version 330

//uniform vec4 uLightPosition;
uniform vec4 uEyePosition;

in vec4 oNormal;
in vec4 oSurfacePosition;

struct LightProperties {
	vec4 Position;
	vec3 AmbientLight;
	vec3 DiffuseLight;
	vec3 SpecularLight;
};

uniform LightProperties uLight;
	
struct MaterialProperties {
	vec3 AmbientReflectivity;
	vec3 DiffuseReflectivity;
	vec3 SpecularReflectivity;
	float Shininess;
};

uniform MaterialProperties uMaterial;

out vec4 FragColour;

void main()
{
	vec4 lightDir = normalize(uLight.Position - oSurfacePosition);
	vec4 eyeDirection = normalize(uEyePosition - oSurfacePosition);
	vec4 reflectedVector = reflect(-lightDir, oNormal);

	float specularFactor = pow(max(dot( reflectedVector, eyeDirection), 0.0), uMaterial.Shininess);
	float diffuseFactor = max(dot(oNormal, lightDir), 0);
	float ambientFactor = 0.05f;

	FragColour = vec4(uLight.AmbientLight * uMaterial.AmbientReflectivity +
		uLight.DiffuseLight * uMaterial.DiffuseReflectivity * diffuseFactor +
		uLight.SpecularLight * uMaterial.SpecularReflectivity * specularFactor, 1);
}