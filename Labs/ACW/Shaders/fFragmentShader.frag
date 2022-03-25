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

vec4 CalculateDirectionalLight(vec4 pNormal, vec4 pEyePos, vec3 pSpecular, float pShininess)
{
	vec3 normal = normalize(vec3(pNormal));
	vec3 eyeNormal = normalize(vec3(uEyePosition));
	float intensity = max(dot(normal, uLightDirection), 0);

	if (intensity > 0.0) 
	{     
        vec3 halfVector = normalize(uLightDirection + eyeNormal);  
        float intSpec = max(dot(halfVector, normal), 0.0);
        return vec4(pSpecular * pow(intSpec, pShininess), 1);
    }
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
		vec4 directionalLight = CalculateDirectionalLight(oNormal, uEyePosition, specularLight, uMaterial.Shininess);

		// Total the light
		vec4 totalLight = vec4(ambientLight + diffuseLight + specularLight, 1);
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