#version 330

uniform mat4 uModel;
uniform mat4 uView;
uniform mat4 uProjection;

in vec3 vPosition; 

out vec4 oColour;

void main() 
{ 
	gl_Position = vec4(vPosition, 1) * uModel * uView * uProjection; 
	oColour = vec4(0, 0, 0, 1);
}