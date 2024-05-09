using System.Text;

namespace Sour;

public sealed class CircularBuffer<T>
{
	public List<T> Buffer => _buffer;
	private List<T> _buffer;
	private int _cursor;
	private readonly int _size;

	public CircularBuffer( int size )
	{
		_size = size;
		_buffer = new( _size );
		_cursor = 0;
	}

	public void Add( T item )
	{
		_buffer.Insert( _cursor, item );
		_cursor += 1;

		if ( _cursor >= _size )
			_cursor = 0;
	}

	public void Clear()
	{
		_buffer.Clear();
		_cursor = 0;
	}

	public override string ToString()
	{
		var sb = new StringBuilder();
		sb.Append( "Buffer contents: " );
		var idx = 0;
		foreach ( var element in _buffer )
		{
			if ( element is null )
				continue;

			sb.Append( string.Format( "{0}: ", idx ) );
			sb.Append( element.ToString() );
			sb.Append( "\n" );

			idx += 1;
		}

		return sb.ToString();
	}
}
