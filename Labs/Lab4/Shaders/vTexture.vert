#version 330

in vec2 vPosition;
in vec2 vTexCoords;

out vec2 oTexCoords;

void main()
{
	oTexCoords = vTexCoords;
	gl_Position = vec4(vPosition, 0, 1);
}