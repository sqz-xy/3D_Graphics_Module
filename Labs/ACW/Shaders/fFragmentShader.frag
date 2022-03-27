#version 330

uniform sampler2D uTextureSampler1;
uniform sampler2D uTextureSampler2;
uniform vec4 uLightPosition;
uniform vec4 uEyePosition;
uniform vec3 uLightDirection;
uniform int uTextureIndex;

in vec4 oNormal;
in vec4 oSurfacePosition;
in vec2 oTexCoords;

out vec4 FragColour;

struct LightProperties {
	vec4 Position;
	vec3 AmbientLight;
	vec3 DiffuseLight;
	vec3 SpecularLight;
};

uniform LightProperties uLight[4];

struct MaterialProperties {
	vec3 AmbientReflectivity;
	vec3 DiffuseReflectivity;
	vec3 SpecularReflectivity;
	float Shininess;
};

uniform MaterialProperties uMaterial;

struct DirectionalLightProperties {
	vec4 Direction;
	vec3 AmbientLight;
	vec3 DiffuseLight;
	vec3 SpecularLight;
};

uniform DirectionalLightProperties uDirectionalLight;

vec4 CalculateDirectionalLight(vec4 pEyeDirection)
{
	vec4 lightDir = normalize(-uDirectionalLight.Direction);
	vec4 reflectedVector = reflect(-lightDir, oNormal);
	float diffuseFactor = max(dot(oNormal, uDirectionalLight.Direction), 0);
	float specularFactor = pow(max(dot(reflectedVector, pEyeDirection), 0.0), uMaterial.Shininess);

	vec3 diffuseLight = diffuseFactor * uDirectionalLight.DiffuseLight * uMaterial.DiffuseReflectivity;
	vec3 specularLight = specularFactor * uDirectionalLight.SpecularLight * uMaterial.SpecularReflectivity;
	vec3 ambientLight = uDirectionalLight.AmbientLight * uMaterial.AmbientReflectivity;
	return vec4(diffuseLight + specularLight + ambientLight, 1);
}

void main()
{
	FragColour = vec4(0.0);
	// Inefficient as you are constantly iterating through per fragment
	for(int i = 0; i < uLight.length(); ++i)
	{
		vec4 eyeDirection = normalize(uEyePosition - oSurfacePosition);
		vec4 lightDir = normalize(uLight[i].Position - oSurfacePosition);
		vec4 reflectedVector = reflect(-lightDir, oNormal);

		float dist = distance(uLight[i].Position, oSurfacePosition);

		// Attenuation
		float attenuation = (1.0 / (1.0 + 0.1 * dist + 0.01 * dist * dist));

		// Lighting factors
		float diffuseFactor = max(dot(oNormal, lightDir), 0);
		float specularFactor = pow(max(dot( reflectedVector, eyeDirection), 0.0), uMaterial.Shininess);

		// Calculate the point light
		vec3 ambientLight = uLight[i].AmbientLight * uMaterial.AmbientReflectivity;
		vec3 diffuseLight = uLight[i].DiffuseLight * uMaterial.DiffuseReflectivity * diffuseFactor;
		vec3 specularLight = uLight[i].SpecularLight * uMaterial.SpecularReflectivity * specularFactor;

		// Calculate the directional Light // DOESNT WORK
		vec4 directionalLight = CalculateDirectionalLight(eyeDirection);

		// Total the light
		vec4 totalLight = vec4(ambientLight + diffuseLight + specularLight, 1) + directionalLight;
		vec4 totalLightAtten = totalLight * attenuation;

		// If no Texture Coords are present
		if (oTexCoords.xy == vec2(0, 0))
			FragColour = FragColour + totalLightAtten;
		else
			// Check the current texture index
			if (uTextureIndex == 0)
				FragColour = FragColour + totalLightAtten * texture(uTextureSampler1, oTexCoords);
			else if (uTextureIndex == 1)
				FragColour = FragColour + totalLightAtten * texture(uTextureSampler2, oTexCoords);
	}			
}