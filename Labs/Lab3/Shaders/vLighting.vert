#version 330

uniform mat4 uModel;
uniform mat4 uView;
uniform mat4 uProjection;
uniform vec3 uLightDirection;

in vec3 vPosition; 
in vec3 vNormal;

out vec4 oColour;

void main() 
{ 
	gl_Position = vec4(vPosition, 1) * uModel * uView * uProjection; 
	oColour = vec4(vec3(max(dot(vNormal, -uLightDirection), 0)), 1);
}