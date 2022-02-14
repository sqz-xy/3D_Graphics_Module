#version 330

uniform mat4 uModel;

in vec3 vPosition;
in vec3 vColour;

out vec4 oColour;

void main()
{
	gl_Position = vec4(vPosition, 1) * uModel;
	oColour = vec4(vColour, 1);
}